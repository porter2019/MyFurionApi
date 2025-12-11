namespace MyFurionApi.Application;

/// <summary>
/// 系统角色组服务
/// </summary>
public class SysRoleService : ISysRoleService, ITransient
{
    private readonly ILogger<SysRoleService> _logger;
    private readonly SqlSugarRepository<SysRole> _sysRoleRepository;

    public SysRoleService(ILogger<SysRoleService> logger, SqlSugarRepository<SysRole> sysRoleRepository)
    {
        _logger = logger;
        _sysRoleRepository = sysRoleRepository;
    }

    /// <summary>
    /// 根据用户id获取所有的权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<string>> GetPermissionsByUserId(int userId)
    {
        var roleList = await GetRoleListByUserIdAsync(userId);
        if (roleList.IsEmpty()) return [];

        if (roleList.Any(p => p.IsSuper)) return ["ALL"];

        var roleIds = roleList.Select(p => p.Id).ToList().Join();

        return await GetPermissionsByRoleIdAsync(roleIds);
    }

    /// <summary>
    /// 根据用户id获取所属的用户组
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<SysRole>> GetRoleListByUserIdAsync(int userId)
    {
        string sql = string.Format(@"select b.* from SysRoleUser as a inner join SysRole as b on a.RoleId = b.Id where a.UserId = {0} and b.IsDeleted = 0", userId);
        return _sysRoleRepository.Ado.SqlQueryAsync<SysRole>(sql);
    }

    /// <summary>
    /// 根据角色ids获取所拥有的权限
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds)
    {
        var dbType = _sysRoleRepository.Context.CurrentConnectionConfig.DbType;
        string concatFunc;

        switch (dbType)
        {
            case DbType.SqlServer:
                concatFunc = "(c.AliasName + '.' + b.AliasName)";
                break;
            case DbType.PostgreSQL:
                concatFunc = "CONCAT(c.AliasName, '.', b.AliasName)";
                break;
            case DbType.MySql:
                concatFunc = "CONCAT(c.AliasName, '.', b.AliasName)";
                break;
            case DbType.Sqlite:
                concatFunc = "(c.AliasName || '.' || b.AliasName)";
                break;
            default:
                concatFunc = "CONCAT(c.AliasName, '.', b.AliasName)";
                break;
        }

        string sql = string.Format(@"SELECT DISTINCT {0} AS auth FROM
                                SysRolePermit AS a 
                                LEFT JOIN SysPermit AS b ON a.PermitId = b.Id AND a.IsDeleted = 0 AND a.RoleId IN({1})
                                LEFT JOIN SysHandler AS c ON b.HandlerId = c.Id AND c.IsDeleted = 0",
                                    concatFunc, roleIds);

        return _sysRoleRepository.Ado.SqlQueryAsync<string>(sql);
    }

    /// <summary>
    /// 根据角色ids和handler信息获取所拥有的权限信息
    /// </summary>
    /// <param name="roleIds"></param>
    /// <param name="refControllerName"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdsAndRefControllerAsync(string roleIds, string refControllerName)
    {
        var dbType = _sysRoleRepository.Context.CurrentConnectionConfig.DbType;
        string quoteChar = dbType == DbType.SqlServer ? "[" : "\"";
        string handlerStatusField = $"{quoteChar}Status{quoteChar}";

        string sql = @$"SELECT DISTINCT B.PermitName FROM(
                        (SELECT * FROM SysRolePermit WHERE IsDeleted = 0 AND RoleId IN ({roleIds})) AS A
                        INNER JOIN
                        (SELECT * FROM SysPermit WHERE IsDeleted = 0 AND HandlerId = (SELECT Id FROM SysHandler WHERE {handlerStatusField} = 1 AND IsDeleted = 0 AND RefController = @refControllerName)) AS B
                        ON A.PermitId = B.Id
                        )";

        return _sysRoleRepository.Ado.SqlQueryAsync<string>(sql, new { refControllerName = refControllerName });
    }

    /// <summary>
    /// 获取用户角色的权限配置信息
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public List<Dto.SysRoleModuleGroupOutput> GetPermitListByRoleId(int roleId)
    {
        var dbType = _sysRoleRepository.Context.CurrentConnectionConfig.DbType;
        string nullFunc;

        switch (dbType)
        {
            case DbType.SqlServer:
                nullFunc = "ISNULL";
                break;
            case DbType.PostgreSQL:
            case DbType.MySql:
            case DbType.Sqlite:
            default:
                nullFunc = "COALESCE";
                break;
        }

        var sql = string.Format(@"SELECT sm.""ModuleName"", sh.""HandlerName"", sp.""Id"" AS ""PermitId"", sp.""PermitName"", sp.""AliasName"", sh.""OrderNo"",
                                            CASE WHEN {0}(srp.""PermitId"", 0) != 0 THEN 1 ELSE 0 END AS ""IsChecked""
                                            FROM ""SysPermit"" AS sp
                                            INNER JOIN ""SysHandler"" AS sh ON sp.""HandlerId"" = sh.""Id""
                                            INNER JOIN ""SysModule"" AS sm ON sh.""ModuleId"" = sm.""Id""
                                            LEFT JOIN ""SysRolePermit"" AS srp ON srp.""PermitId"" = sp.""Id"" AND srp.""RoleId"" = {1}",
                                                nullFunc, roleId);

        var _sysRolePermitRepo = _sysRoleRepository.Change<SysRolePermit>();
        var dbData = _sysRolePermitRepo.Ado.SqlQuery<Dto.SysRolePermitOutput>(sql);

        var moduleGroup = dbData.GroupBy(p => p.ModuleName).ToList();
        var resultList = new List<Dto.SysRoleModuleGroupOutput>();

        foreach (var module in moduleGroup)
        {
            var moduleModel = new Dto.SysRoleModuleGroupOutput(module.Key);
            var handlerGroup = dbData.Where(o => o.ModuleName == module.Key).ToList().GroupBy(o => o.HandlerName).ToList();

            foreach (var handler in handlerGroup)
            {
                var handlerModel = new Dto.SysRoleHandlerGroupOutput(handler.Key);
                handlerModel.PermitList = handler.OrderByDescending(p => p.OrderNo).ToList();
                moduleModel.HandlerList.Add(handlerModel);
            }
            resultList.Add(moduleModel);
        }

        return resultList;
    }
    /// <summary>
    /// 设置用户组权限
    /// </summary>
    /// <param name="roleId">组id</param>
    /// <param name="permits">权限PermitId，英文逗号分开</param>
    /// <returns></returns>
    public async Task<bool> SetRolePermitAsync(int roleId, string permits)
    {
        var rolePermitList = new List<SysRolePermit>();
        foreach (var item in permits.SplitWithComma())
        {
            rolePermitList.Add(new SysRolePermit()
            {
                PermitId = item.ObjToInt(),
                RoleId = roleId
            });
        }

        var _rolePermitRepo = _sysRoleRepository.Change<SysRolePermit>();

        await _rolePermitRepo.DeleteAsync(x => x.RoleId == roleId);
        await _rolePermitRepo.InsertAsync(rolePermitList);

        return true;
    }

}
