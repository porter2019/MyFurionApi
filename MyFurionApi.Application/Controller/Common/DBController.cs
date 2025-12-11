using System.Reflection;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 数据库
/// </summary>
[AllowAnonymous]
public class DBController : BaseApiController
{
    private readonly ILogger<DBController> _logger;
    private readonly ISqlSugarClient _db;
    private readonly SqlSugarRepository<SysHandler> _handlerRepo;

    public DBController(ILogger<DBController> logger, ISqlSugarClient db, SqlSugarRepository<SysHandler> handlerRepo)
    {
        _logger = logger;
        _db = db;
        _handlerRepo = handlerRepo;
    }

    /// <summary>
    /// 数据库迁移
    /// </summary>
    /// <param name="tableNames">指定实体名称，英文逗号分割</param>
    /// <returns></returns>
    [HttpGet]
    public string Migration(string tableNames)
    {
        _logger.LogDebug("======数据库迁移开始======");
        _db.DbMaintenance.CreateDatabase();
        _logger.LogDebug($"数据库创建完成，如果是新建，注意修改数据库排序规则为：Chinese_PRC_CI_AS");
        //sql server下修改数据库排序规则为：Chinese_PRC_CI_AS
        //ALTER DATABASE 数据库名称 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        //ALTER DATABASE 数据库名称 COLLATE Chinese_PRC_CI_AS
        //ALTER DATABASE 数据库名称 SET MULTI_USER

        //mysql8下修改数据库排序规则为：utf8mb4_0900_ai_ci

        var tableNameList = tableNames.SplitWithComma();
        List<Type> types = App.EffectiveTypes.Where(a => !a.IsAbstract && a.IsClass && a.GetCustomAttributes(typeof(FsTableAttribute), true)?.FirstOrDefault() != null).ToList();
        foreach (var type in types)
        {
            var fsTableAttr = type.GetCustomAttribute(typeof(FsTableAttribute), true) as FsTableAttribute;
            if (fsTableAttr.IsIgnore)
            {
                _logger.LogDebug($"已忽略实体【{type.FullName}】");
                continue;
            }

            if (tableNameList.Any())
            {
                if (tableNameList.Contains(type.Name))
                {
                    _db.CodeFirst.InitTables(type);
                    _logger.LogDebug($"指定了实体名称【{type.Name}】，已迁移实体【{type.FullName}】");
                }
            }
            else
            {

                _db.CodeFirst.InitTables(type);
                _logger.LogDebug($"已迁移实体【{type.FullName}】");
            }
        }

        _logger.LogDebug("======数据库迁移结束======");
        return "OK";
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
        var delAffrows = _sysModuleRepo.Delete(x => !moduleIdList.Contains(x.Id));
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
                        _sysPermitRepo.Update(x => new SysPermit() { AliasName = opeartion.Value }, x => x.Id == permitId);
                        _logger.LogInformation($"【权限数据初始化】修改修改权限别名：{opeartion}，新的别名：{opeartion.Value}，所属模块{moduleName},命名空间：{controllerFullName}");
                    }
                }
                permitIdList.Add(permitId);
            }
        }

        //删除冗余权限数据
        var delHandlerCount = _sysHandlerRepo.Delete(x => !handlerIdList.Contains(x.Id));
        var delPermitCount = _sysPermitRepo.Delete(x => !permitIdList.Contains(x.Id));
        var delRolePermitCount = _sysRolePermitRepo.Delete(x => !permitIdList.Contains(x.PermitId));
        _logger.LogInformation($"【权限数据初始化】清除冗余数据：功能清除{delHandlerCount}条，权限清除{delPermitCount}条，用户组权限清除{delRolePermitCount}条");


        return "更新完毕";
    }

    /// <summary>
    /// 执行数据库脚本
    /// </summary>
    /// <returns></returns>
    [HttpGet, SwaggerIgnore]
    [Obsolete("不用这种方式")]
    public string SqlScript()
    {
        //先刷新视图
        var refreshViewSql = "select TABLE_NAME from INFORMATION_SCHEMA.VIEWS order by TABLE_NAME";
        var viewNameList = _db.Ado.SqlQuery<string>(refreshViewSql);
        foreach (var viewName in viewNameList)
        {
            _db.Ado.ExecuteCommand($"exec sp_refreshview {viewName}");
            _logger.LogDebug($"刷新视图：{viewName}");
        }
        //获取启动项根目录 var webRootPath = App.HostEnvironment.ContentRootPath;
        //获取网站根目录wwwroot目录 var wwwroot = App.WebHostEnvironment.WebRootPath;

        var sqlScriptFolder = Path.Combine(App.WebHostEnvironment.WebRootPath, "content", "sqlscript");
        var sqlFileList = Directory.GetFiles(sqlScriptFolder, "*.sql", SearchOption.AllDirectories);
        foreach (var sqlFilePath in sqlFileList)
        {
            var sqlViewText = File.ReadAllText(sqlFilePath);
            if (sqlViewText.IsNull()) continue;

            var arrSqlView = sqlViewText.Split(new string[2]{
                                    "\r\ngo\r\n",
                                    "\r\nGO\r\n"
                                }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sqlItem in arrSqlView)
            {
                if (string.IsNullOrEmpty(sqlItem)) continue;
                string execSql = sqlItem;
                try
                {
                    if (sqlItem.EndsWith("go") || sqlItem.EndsWith("GO"))
                    {
                        execSql = sqlItem.Substring(0, sqlItem.Length - 2);
                    }
                    _db.Ado.ExecuteCommand(execSql);
                }
                catch (Exception e)
                {
                    _logger.LogError($"执行Sql脚本文件出错，文件：{sqlFilePath}，错误信息：{e.Message}");
                    continue;
                }
            }
        }
        return "OK";
    }

}
