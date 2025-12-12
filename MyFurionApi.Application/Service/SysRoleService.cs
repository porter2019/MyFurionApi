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
        return _sysRoleRepository.AsQueryable()
            .InnerJoin<SysRoleUser>((a, b) => b.RoleId == a.Id)
            .Where((a, b) => b.UserId == userId)
            .Select(a => a)
            .ToListAsync();
    }

    /// <summary>
    /// 根据角色ids获取所拥有的权限
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds)
    {
        var roleIdList = roleIds.SplitWithComma().ConvertIntArray();
        return _sysRoleRepository.Change<SysRolePermit>().AsQueryable()
                .LeftJoin<SysPermit>((a, b) => a.PermitId == b.Id && !a.IsDeleted)
                .LeftJoin<SysHandler>((a, b, c) => b.HandlerId == c.Id && !c.IsDeleted)
                .Where((a, b, c) => roleIdList.Contains(a.RoleId))
                .Select((a, b, c) => SqlFunc.MergeString(c.AliasName, ".", b.AliasName))
                .Distinct()
                .ToListAsync();
    }

    /// <summary>
    /// 根据角色ids和handler信息获取所拥有的权限信息
    /// </summary>
    /// <param name="roleIds"></param>
    /// <param name="refControllerName"></param>
    /// <returns></returns>
    public Task<List<string>> GetPermissionsByRoleIdsAndRefControllerAsync(string roleIds, string refControllerName)
    {
        var roleIdsArray = roleIds.SplitWithComma().ConvertIntArray();

        return _sysRoleRepository.Change<SysRolePermit>().AsQueryable()
                .Where(a => !a.IsDeleted && roleIdsArray.Contains(a.RoleId))
                .InnerJoin<SysPermit>((a, b) => a.PermitId == b.Id && !b.IsDeleted)
                .InnerJoin<SysHandler>((a, b, c) => b.HandlerId == c.Id && c.Status && !c.IsDeleted && c.RefController == refControllerName)
                .Select((a, b, c) => b.PermitName)
                .Distinct()
                .ToListAsync();
    }

    /// <summary>
    /// 获取用户角色的权限配置信息
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<List<Dto.SysRoleModuleGroupOutput>> GetPermitListByRoleId(int roleId)
    {
        var dbData = await _sysRoleRepository.Change<SysPermit>().AsQueryable()
                .InnerJoin<SysHandler>((sp, sh) => sp.HandlerId == sh.Id && !sh.IsDeleted)
                .InnerJoin<SysModule>((sp, sh, sm) => sh.ModuleId == sm.Id && !sm.IsDeleted)
                .LeftJoin<SysRolePermit>((sp, sh, sm, srp) => srp.PermitId == sp.Id && srp.RoleId == roleId && !srp.IsDeleted)
                .Where(sp => !sp.IsDeleted)
                .Select((sp, sh, sm, srp) => new Dto.SysRolePermitOutput
                {
                    ModuleName = sm.ModuleName,
                    HandlerName = sh.HandlerName,
                    PermitId = sp.Id,
                    PermitName = sp.PermitName,
                    AliasName = sp.AliasName,
                    OrderNo = sh.OrderNo,
                    IsChecked = SqlFunc.IIF(SqlSugar.SqlFunc.IsNull(srp.PermitId, 0) == 0, false, true)
                })
                .ToListAsync();

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
