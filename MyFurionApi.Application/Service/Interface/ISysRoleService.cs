namespace MyFurionApi.Application;

public interface ISysRoleService
{
    Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds);
    Task<List<string>> GetPermissionsByRoleIdsAndRefControllerAsync(string roleIds, string refControllerName);
    List<SysRoleModuleGroupOutput> GetPermitListByRoleId(int roleId);
    Task<List<SysRole>> GetRoleListByUserIdAsync(int userId);
    Task<bool> SetRolePermitAsync(int roleId, string permits);
}
