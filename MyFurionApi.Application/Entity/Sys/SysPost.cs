namespace MyFurionApi.Application.Entity;

/// <summary>
/// 岗位
/// </summary>
[FsTable]
public class SysPost : BaseEntityStandard
{
    /// <summary>
    /// 岗位编码
    /// </summary>
    [FsColumn(50)]
    public string Code { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    [FsColumn(50)]
    public string Name { get; set; }

    /// <summary>
    /// 排序数字
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int Sort { get; set; }
}
