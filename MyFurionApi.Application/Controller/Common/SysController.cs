using System.Reflection;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 系统
/// </summary>
public class SysController : BaseApiController
{
    private readonly ILogger<SysController> _logger;
    private readonly IConfiguration _config;
    private readonly SqlSugarRepository<SysHandler> _handlerRepo;

    public SysController(ILogger<SysController> logger, IConfiguration config, SqlSugarRepository<SysHandler> handlerRepo)
    {
        _logger = logger;
        _config = config;
        _handlerRepo = handlerRepo;
    }

    /// <summary>
    /// 获取系统配置文件中的值
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public string GetConfigValue(string path)
    {
        if (path.IsNull()) return "path不能为空";
        return _config[path];
    }

    /// <summary>
    /// 获取IP地址
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public string GetIP()
    {
        var local = App.HttpContext.GetLocalIpAddressToIPv4();
        var remote = App.HttpContext.GetRemoteIpAddressToIPv4();
        return $"本机IP：{local}；客户端IP：{remote}；当前时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
    }

    /// <summary>
    /// 更新系统权限
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [UnitOfWork]
    public string SyncPermis()
    {
        var _sysModuleRepo = _handlerRepo.Change<SysModule>();
        var _sysHandlerRepo = _handlerRepo.Change<SysHandler>();
        var _sysPermitRepo = _handlerRepo.Change<SysPermit>();
        var _sysRolePermitRepo = _handlerRepo.Change<SysRolePermit>();

        var types = App.EffectiveTypes;

        Func<Attribute[], bool> isAttribute = o =>
        {
            return o.OfType<PermissionHandlerAttribute>().Any();
        };

        Type[] controllerList = types.Where(o => isAttribute(Attribute.GetCustomAttributes(o, true)))
                .OrderBy(o => (o.GetCustomAttributes(typeof(PermissionHandlerAttribute), true)[0] as PermissionHandlerAttribute).OrderNo).ToArray();

        //模块Id集合
        List<int> moduleIdList = new();
        //功能Id集合
        List<int> handlerIdList = new();
        //权限Id集合
        List<int> permitIdList = new();

        //数据库已有的模块
        var dbModuleList = _sysModuleRepo.ToList();
        //获取所有模块名称
        var registeredModuleList = controllerList
                                        .Select(p => p.GetCustomAttributes(typeof(PermissionHandlerAttribute), true)[0] as PermissionHandlerAttribute)
                                        .OrderByDescending(p => p.OrderNo)
                                        .ToList()
                                        .Select(p => p.ModuleName).Distinct().ToList();//去重
        foreach (var moduleName in registeredModuleList)
        {
            var moduleId = 0;
            var entityModule = dbModuleList.Where(p => p.ModuleName == moduleName).FirstOrDefault();
            if (entityModule == null)
            {
                entityModule = new SysModule(moduleName);
                moduleId = _sysModuleRepo.InsertReturnIdentity(entityModule);
                _logger.LogInformation($"【权限数据初始化】新增模块:{moduleName},数据编号:{moduleId}");
            }
            else moduleId = entityModule.Id;

            moduleIdList.Add(moduleId);
        }

        //删除冗余模块数据
        var delAffrows = _sysModuleRepo.Delete($"Id not in ({moduleIdList.Join()})");
        _logger.LogInformation($"【权限数据初始化】清除冗余模块，删除{delAffrows}条");

        //重新查找数据库现有的模块
        dbModuleList = _sysModuleRepo.ToList();

        //功能集合
        var dbHandlerList = _sysHandlerRepo.ToList();
        //权限集合
        var dbPermitList = _sysPermitRepo.ToList();

        foreach (var controllerInfo in controllerList)
        {
            var handlerAttribute = (controllerInfo.GetCustomAttributes(typeof(PermissionHandlerAttribute), true)[0] as PermissionHandlerAttribute);
            if (handlerAttribute == null) continue;

            //控制器名称
            var controllerFullName = controllerInfo.FullName;
            var moduleName = handlerAttribute.ModuleName;
            if (moduleName.IsNull()) continue;
            var handerName = handlerAttribute.HandlerName;
            if (handerName.IsNull()) continue;
            var handerAliasName = handlerAttribute.AliasName;
            if (handerAliasName.IsNull()) continue;

            var moduleEntity = dbModuleList.Where(p => p.ModuleName == moduleName).FirstOrDefault();
            if (moduleEntity == null) continue;

            var newHandler = new SysHandler()
            {
                ModuleId = moduleEntity.Id,
                HandlerName = handerName,
                AliasName = handerAliasName,
                OrderNo = handlerAttribute.OrderNo,
                RefController = controllerFullName
            };

            //功能
            var handerId = 0;
            var entityHandler = dbHandlerList.Where(p => p.RefController == controllerFullName).FirstOrDefault();
            if (entityHandler == null)
            {
                handerId = _sysHandlerRepo.InsertReturnIdentity(newHandler);
                _logger.LogInformation($"【权限数据初始化】新增功能：{handerName}，所属模块：{moduleName}，命名空间：{controllerFullName}");
            }
            else
            {
                handerId = entityHandler.Id;
                newHandler.Id = entityHandler.Id;
                var affrows = _sysHandlerRepo.Update(newHandler);
                if (affrows > 0)
                    _logger.LogInformation($"【权限数据初始化】修改功能：{handerName}，所属模块：{moduleName},命名空间：{controllerFullName}");
            }
            handlerIdList.Add(handerId);

            //权限
            var methodList = controllerInfo.GetMethods();
            //获取所有权限
            Dictionary<string, string> opeartionNameList = new Dictionary<string, string>();
            foreach (var methodInfo in methodList)
            {
                var permissionAttr = methodInfo.GetCustomAttribute<PermissionAttribute>(true);
                if (permissionAttr == null) continue;

                string[] operations = permissionAttr.OperationName.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in operations)
                {
                    if (!opeartionNameList.ContainsKey(item)) opeartionNameList.Add(item, permissionAttr.AliasName);
                }
            }

            foreach (var opeartion in opeartionNameList)
            {
                int permitId = 0;
                var entityPermit = dbPermitList.Where(p => p.HandlerId == handerId && p.PermitName == opeartion.Key).FirstOrDefault();// && p.AliasName == opeartion.Value
                if (entityPermit == null)
                {
                    entityPermit = new SysPermit()
                    {
                        HandlerId = handerId,
                        PermitName = opeartion.Key,
                        AliasName = opeartion.Value
                    };
                    permitId = _sysPermitRepo.InsertReturnIdentity(entityPermit);
                    _logger.LogInformation($"【权限数据初始化】新增权限：{opeartion}，所属功能:{handerName}，所属模块:{moduleName}");
                }
                else
                {
                    permitId = entityPermit.Id;
                    if (!entityPermit.AliasName.Equals(opeartion.Value))
                    {
                        //如果改了别名，则只改别名就行了
                        _sysPermitRepo.EntityContext.Updateable<SysPermit>().SetColumns(x => x.AliasName == opeartion.Value).Where(x => x.Id == permitId).ExecuteCommand();
                        _logger.LogInformation($"【权限数据初始化】修改修改权限别名：{opeartion}，新的别名：{opeartion.Value}，所属模块{moduleName},命名空间：{controllerFullName}");
                    }
                }
                permitIdList.Add(permitId);
            }
        }

        //删除冗余权限数据
        var delHandlerCount = _sysHandlerRepo.Delete($"Id not in ({handlerIdList.Join()})");
        var delPermitCount = _sysPermitRepo.Delete($"Id not in ({permitIdList.Join()})");
        var delRolePermitCount = _sysRolePermitRepo.Delete($"PermitId not in ({permitIdList.Join()})");
        _logger.LogInformation($"【权限数据初始化】清除冗余数据：功能清除{delHandlerCount}条，权限清除{delPermitCount}条，用户组权限清除{delRolePermitCount}条");


        return "更新完毕";
    }

}
