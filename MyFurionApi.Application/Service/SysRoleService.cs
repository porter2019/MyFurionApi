namespace MyFurionApi.Application;

/// <summary>
/// 系统角色组服务
/// </summary>
public class SysRoleService : ISysRoleService, ITransient
{
    private readonly ILogger<ProductService> _logger;
    private readonly SqlSugarRepository<SysRole> _sysRoleRepo;

    public SysRoleService(ILogger<ProductService> logger, SqlSugarRepository<SysRole> sysRoleRepo)
    {
        _logger = logger;
        _sysRoleRepo = sysRoleRepo;
    }

    /// <summary>
    /// 根据用户id获取所属的用户组
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<List<SysRole>> GetRoleListByUserIdAsync(int userId)
    {
        string sql = string.Format(@"select b.* from SysRoleUser as a inner join SysRole as b on a.Id = b.Id and a.UserId = {0} and b.IsDeleted = 0", userId);
        return _sysRoleRepo.Ado.SqlQueryAsync<SysRole>(sql);
    }

    /// <summary>
    /// 根据组ids获取所拥有的权限
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds)
    {
        string sql = string.Format(@"select DISTINCT (c.AliasName + '.' + b.AliasName) as auth from
                                            SysRolePermit as a left join SysPermit as b on a.PermitId = b.Id and a.IsDeleted = 0 and a.RoleId in({0})
                                            left join SysHandler as c on b.HandlerId = c.Id and c.IsDeleted=0", roleIds);
        return _sysRoleRepo.Ado.SqlQueryAsync<string>(sql);
    }

    /// <summary>
    /// 根据组ids和handler信息获取所拥有的权限信息
    /// </summary>
    /// <param name="roleIds"></param>
    /// <param name="refControllerName"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdsAndRefControllerAsync(string roleIds, string refControllerName)
    {
        string sql = @$"select distinct B.PermitName from(
                            (select * from SysRolePermit where IsDeleted = 0 and RoleId in ({roleIds})) as A
                            inner join
                            (select * from SysPermit where IsDeleted = 0 and HandlerId = (select Id from SysHandler where [Status] = 1 and IsDeleted = 0 and RefController = '{refControllerName}')) as B
                            on A.PermitId = B.Id
                            )";

        return _sysRoleRepo.Ado.SqlQueryAsync<string>(sql);
    }

    /// <summary>
    /// 获取用户组的权限配置信息
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public List<Dto.SysRoleModuleGroupOutput> GetPermitListByRoleId(int roleId)
    {
        var sql = string.Format(@"select sm.ModuleName, sh.HandlerName, sp.Id as PermitId,sp.PermitName, sp.AliasName, sh.OrderNo,
                                                case when ISNULL(srp.PermitId,0)!=0  then 1 else 0 end IsChecked
                                                from SysPermit as sp
                                                inner join SysHandler as sh on sp.HandlerId = sh.Id
                                                inner join SysModule as sm on sh.ModuleId = sm.Id
                                                left join SysRolePermit as srp on srp.PermitId = sp.Id and srp.RoleId = {0}",
                                        roleId);
        var _sysRolePermitRepo = _sysRoleRepo.Change<SysRolePermit>();

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

        var _rolePermitRepo = _sysRoleRepo.Change<SysRolePermit>();

        try
        {
            _rolePermitRepo.CurrentBeginTran();

            await _rolePermitRepo.DeleteAsync(x => x.RoleId == roleId);
            await _rolePermitRepo.InsertAsync(rolePermitList);

            _rolePermitRepo.CurrentCommitTran();

            return true;
        }
        catch
        {
            _rolePermitRepo.CurrentRollbackTran();
            return false;
        }

    }

}
