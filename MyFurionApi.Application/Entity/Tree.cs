namespace MyFurionApi.Application.Entity;

/// <summary>
/// 树型实体
/// </summary>
[FsTable()]
public class Tree : BaseEntityStandard
{
    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn()]
    public string Name { get; set; }

    /// <summary>
    /// 排序数字，这里是string类型的，升序排列
    /// </summary>
    [FsColumn(1000)]
    public string OrderNo { get; set; } = "01";

    /// <summary>
    /// 上级Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int ParentId { get; set; }

    /// <summary>
    /// 父级名称
    /// </summary>
    [FsColumn()]
    public string ParentName { get; set; }

    /// <summary>
    /// 完整Id
    /// </summary>
    [FsColumn(1000)]
    public string FullId { get; set; }

    /// <summary>
    /// 完整名称
    /// </summary>
    [FsColumn(1000)]
    public string FullName { get; set; }

    /// <summary>
    /// 完整类别层级排序，竖线分割
    /// </summary>
    [FsColumn(1000)]
    public string FullOrderNo { get; set; }

    /// <summary>
    /// 层级
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int LevelNo { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool Status { get; set; } = true;

    /// <summary>
    /// 父级信息
    /// </summary>
    [FsColumn(true)]
    [Navigate(NavigateType.OneToOne, nameof(ParentId))]
    public Tree Parent { get; set; }

    /// <summary>
    /// 子列表
    /// </summary>
    [FsColumn(true)]
    public List<Tree> Childs { get; set; }
}


/// <summary>
/// 生成排序号查询时所需实体
/// </summary>
public class TreeGenerateNextNoQuery : BaseSingleQueryModel
{
    /// <summary>
    /// 上级id
    /// </summary>
    [PageQuery(PageQueryOperatorType.Equal)]
    public int ParentId { get; set; }
}
