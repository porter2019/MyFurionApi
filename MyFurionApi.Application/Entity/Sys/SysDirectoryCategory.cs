namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统字典类别
/// </summary>
[FsTable()]
public class SysDirectoryCategory : BaseEntityTree
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
