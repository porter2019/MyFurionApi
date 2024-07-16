namespace MyFurionApi.Application2.Entity;

/// <summary>
/// 演示表-未同步
/// </summary>
[SugarTable(null)]
public class Demo : BaseEntityStandard
{
    /// <summary>
    /// 名称
    /// </summary>
    [FsColumn("名称", true, 255)]
    public string? Name { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [FsColumn("数量")]
    public int Num { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [FsColumn("金额", ColumnDataType = DBColumnDataType.Money)]
    public decimal Value { get; set; }

}
