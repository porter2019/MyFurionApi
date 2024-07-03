namespace MyFurionApi.Application.Entity;

/// <summary>
/// 用户组下的用户
/// </summary>
[SugarTable(null)]
public class SysRoleUser : BaseEntityStandard
{
    /// <summary>
    /// 用户组Id
    /// </summary>
    [FsColumn("用户组Id", false)]
    public int RoleId { set; get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [FsColumn("用户Id", false)]
    public int UserId { set; get; }

}