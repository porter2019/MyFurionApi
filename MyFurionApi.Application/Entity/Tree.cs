namespace MyFurionApi.Application.Entity;

/// <summary>
/// 树型实体
/// </summary>
[FsTable()]
public class Tree : BaseEntityTree
{
    /// <summary>
    /// 标识
    /// </summary>
    [FsColumn(50)]
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn(50)]
    public string Name { get; set; }

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
