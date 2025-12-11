namespace MyFurionApi.Application.Entity;

/// <summary>
/// 用户组权限
/// </summary>
[FsTable()]
public class SysRolePermit : BaseEntity
{
    /// <summary>
    /// 用户组Id
    /// </summary>
    [FsColumn()]
    public int RoleId { get; set; }

    /// <summary>
    /// 权限Id
    /// </summary>
    [FsColumn()]
    public int PermitId { get; set; }
}