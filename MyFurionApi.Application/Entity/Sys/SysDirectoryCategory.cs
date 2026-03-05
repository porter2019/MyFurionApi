namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统字典类别
/// </summary>
[FsTable()]
public class SysDirectoryCategory : BaseEntityStandard
{
    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn()]
    public string Name { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    [FsColumn()]
    public string Code { get; set; }

    /// <summary>
    /// 排序数字
    /// <code>升序排列</code>
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
    /// 完整类别层级排序
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
    public SysDirectoryCategory Parent { get; set; }

    /// <summary>
    /// 子列表
    /// </summary>
    [FsColumn(true)]
    public List<SysDirectoryCategory> Childs { get; set; }
}
