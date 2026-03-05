namespace MyFurionApi.Application.Entity;

/// <summary>
/// 字典项
/// </summary>
[FsTable()]
public class SysDirectoryItem : BaseEntityStandard
{
    /// <summary>
    /// 分类Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int CategoryId { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn(IsNullable = false)]
    public string Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    [FsColumn(IsNullable = false)]
    public string Value { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool Status { get; set; } = true;
}

/// <summary>
/// 字典项全信息
/// </summary>
[FsTable("SysDirectoryItemFullInfoView", IsIgnore = true)]
public class SysDirectoryItemFullInfo : SysDirectoryItem
{
    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; }

    /// <summary>
    /// 分类编码
    /// </summary>
    public string CategoryCode { get; set; }
}
