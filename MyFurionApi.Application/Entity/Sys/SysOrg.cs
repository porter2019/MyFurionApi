namespace MyFurionApi.Application.Entity;

/// <summary>
/// 部门
/// </summary>
[FsTable]
public class SysOrg : BaseEntityTree
{
    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn(50)]
    public string Name { get; set; }

    /// <summary>
    /// 负责人Id
    /// </summary>
    [FsColumn()]
    public int LeaderId { get; set; }

    /// <summary>
    /// 负责人名称
    /// </summary>
    [FsColumn(30)]
    public string LeaderName { get; set; } //冗余存储

    /// <summary>
    /// 父级信息
    /// </summary>
    [FsColumn(true)]
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public SysOrg Parent { get; set; }

    /// <summary>
    /// 子列表
    /// </summary>
    [FsColumn(true)]
    public List<SysOrg> Childs { get; set; }
}
