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
    [FsColumn("用户组Id", true)]
    public int RoleId { set; get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [FsColumn("用户Id", true)]
    public int UserId { set; get; }

}