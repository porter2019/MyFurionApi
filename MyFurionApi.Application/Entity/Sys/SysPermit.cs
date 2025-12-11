namespace MyFurionApi.Application.Entity;

/// <summary>
/// 功能下的权限
/// </summary>
[FsTable()]
public class SysPermit : BaseEntity
{
    /// <summary>
    /// 权限名称
    /// </summary>
    [FsColumn(50)]
    public string PermitName { get; set; }

    /// <summary>
    /// 权限别名
    /// </summary>
    [FsColumn(50)]
    public string AliasName { get; set; }

    /// <summary>
    /// 功能Id
    /// </summary>
    [FsColumn()]
    public int HandlerId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [FsColumn()]
    public bool Status { get; set; } = true;
}