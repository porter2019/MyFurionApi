namespace MyFurionApi.Application.Entity;

/// <summary>
/// 用户组下的用户
/// </summary>
[FsTable()]
public class SysRoleUser : BaseEntityStandard
{
    /// <summary>
    /// 用户组Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int RoleId { set; get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int UserId { set; get; }

}