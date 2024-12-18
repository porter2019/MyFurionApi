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
    [FsColumn("用户组Id", false)]
    public int RoleId { get; set; }

    /// <summary>
    /// 权限Id
    /// </summary>
    [FsColumn("权限Id", false)]
    public int PermitId { get; set; }
}