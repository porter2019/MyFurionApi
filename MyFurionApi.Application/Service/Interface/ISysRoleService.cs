namespace MyFurionApi.Application;

public interface ISysRoleService
{
    /// <summary>
    /// 根据组ids获取所拥有的权限
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds);

    /// <summary>
    /// 根据组ids和handler信息获取所拥有的权限信息
    /// </summary>
    /// <param name="roleIds"></param>
    /// <param name="refControllerName"></param>
    /// <returns></returns>
    Task<List<string>> GetPermissionsByRoleIdsAndRefControllerAsync(string roleIds, string refControllerName);

    /// <summary>
    /// 根据用户id获取所有的权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<string>> GetPermissionsByUserId(int userId);

    /// <summary>
    /// 获取用户组的权限配置信息
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    List<SysRoleModuleGroupOutput> GetPermitListByRoleId(int roleId);

    /// <summary>
    /// 根据用户id获取所属的用户组
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<List<SysRole>> GetRoleListByUserIdAsync(int userId);

    /// <summary>
    /// 设置用户组权限
    /// </summary>
    /// <param name="roleId">组id</param>
    /// <param name="permits">权限PermitId，英文逗号分开</param>
    /// <returns></returns>
    Task<bool> SetRolePermitAsync(int roleId, string permits);
}
