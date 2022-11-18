namespace MyFurionApi.Application.Entity;

/// <summary>
/// 树型实体
/// </summary>
[SugarTable(null)]
public class ProCategory : BaseEntityStandard
{
    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn("名称", false)]
    public string Name { get; set; }

    /// <summary>
    /// 排序数字，这里是string类型的，升序排列
    /// </summary>
    [FsColumn("排序", true, 500)]
    public string OrderNo { get; set; } = "01";

    /// <summary>
    /// 上级Id
    /// </summary>
    [FsColumn("父Id")]
    public int ParentId { get; set; }

    /// <summary>
    /// 父级名称
    /// </summary>
    [FsColumn("父级名称", true)]
    public string ParentName { get; set; }

    /// <summary>
    /// 完整Id
    /// </summary>
    [FsColumn("完整Id", true, 500)]
    public string FullId { get; set; }

    /// <summary>
    /// 完整名称
    /// </summary>
    [FsColumn("完整名称", true, 500)]
    public string FullName { get; set; }

    /// <summary>
    /// 完整类别层级排序，竖线分割
    /// </summary>
    [FsColumn("完整排序", true, 2000)]
    public string FullOrderNo { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    [FsColumn("层级")]
    public int LevelNo { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    [FsColumn("启用状态")]
    public bool Status { get; set; } = true;

    /// <summary>
    /// 父级信息
    /// </summary>
    [FsColumn("父级信息", IsIgnore = true)]
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public ProCategory Parent { get; set; }

    /// <summary>
    /// 子列表
    /// </summary>
    [FsColumn("子列表", IsIgnore = true)]
    public List<ProCategory> Childs { get; set; }
}
