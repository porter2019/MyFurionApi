namespace MyFurionApi.Application;

/// <summary>
/// 用户服务
/// </summary>
public class SysUserService : ISysUserService, ITransient
{
    private readonly ILogger<SysUserService> _logger;
    private readonly ISysRoleService _roleService;
    private readonly SqlSugarRepository<SysUser> _sysUserRepository;

    public SysUserService(ILogger<SysUserService> logger, ISysRoleService roleService, SqlSugarRepository<SysUser> sysUserRepository)
    {
        _logger = logger;
        _roleService = roleService;
        _sysUserRepository = sysUserRepository;
    }

    /// <summary>
    /// 获取有某个权限的用户Id列表
    /// </summary>
    /// <param name="hanlderName">ERPOrderSalesJob1</param>
    /// <param name="actionName">如果为空，则默认置为：show</param>
    /// <returns></returns>
    public Task<List<int>> GetPermitUserIdList(string hanlderName, string actionName)
    {
        if (actionName.IsNull()) actionName = "show";

        var dbType = _sysUserRepository.Context.CurrentConnectionConfig.DbType;
        string handlerTable = dbType == DbType.SqlServer ? "[SysHandler]" : "\"SysHandler\"";
        string permitTable = dbType == DbType.SqlServer ? "[SysPermit]" : "\"SysPermit\"";
        string rolePermitTable = dbType == DbType.SqlServer ? "[SysRolePermit]" : "\"SysRolePermit\"";
        string roleUserTable = dbType == DbType.SqlServer ? "[SysRoleUser]" : "\"SysRoleUser\"";

        var sql = $@"SELECT DISTINCT d.""UserId""
                FROM {roleUserTable} d
                WHERE EXISTS (
                    SELECT 1
                    FROM {rolePermitTable} c
                    INNER JOIN {permitTable} b ON b.""Id"" = c.""PermitId"" 
                        AND b.""AliasName"" = @actionName
                        AND b.""IsDeleted"" = 0
                    INNER JOIN {handlerTable} a ON a.""Id"" = b.""HandlerId"" 
                        AND a.""AliasName"" = @hanlderName
                        AND a.""IsDeleted"" = 0
                    WHERE c.""RoleId"" = d.""RoleId"" 
                        AND c.""IsDeleted"" = 0
                        AND d.""IsDeleted"" = 0
                );";

        var sqlParams = new List<SugarParameter>()
    {
        new("@hanlderName", hanlderName),
        new("@actionName", actionName),
    };
        return _sysUserRepository.Ado.SqlQueryAsync<int>(sql, sqlParams);
    }

    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="controllerName">接口控制器命名空间完整名称</param>
    /// <param name="operations">要校验的操作,英文逗号分割</param>
    /// <returns></returns>
    public async Task<bool> CheckHasPermissionAsync(int userId, string controllerName, string operations)
    {
        var operationList = operations.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        if (!operationList.Any()) throw Oops.Oh("缺少要检验的操作");
        if (controllerName.IsNull()) throw Oops.Oh("缺少控制器名称");

        var userRoleList = await _roleService.GetRoleListByUserIdAsync(userId);
        if (userRoleList.IsEmpty()) throw Oops.Oh("用户没有所在的组，请联系管理员进行分配");
        if (userRoleList.Any(p => p.IsSuper)) return true; //所在组里有超级管理员组的就通过

        var hasPermit = await _roleService.GetPermissionsByRoleIdsAndRefControllerAsync(userRoleList.Select(x => x.Id).ToList().Join(), controllerName);
        //交集
        var intersectList = operationList.Intersect(hasPermit);
        return intersectList.Any() ? true : false;
    }

}
