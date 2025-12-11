namespace MyFurionApi.Application.Entity;

/// <summary>
/// 合同明细
/// </summary>
[FsTable()]
public class ContractItem : BaseEntity
{
    /// <summary>
    /// 所属合同Id
    /// </summary>
    [FsColumn(IsNullable = false), ForeignKeyTag]
    public int ContractId { get; set; }

    /// <summary>
    /// 明细名称
    /// </summary>
    [FsColumn(500)]
    public string Name { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [FsColumn(IsNullable = false, ColumnDataType = DBColumnDataType.Money)]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [FsColumn(IsNullable = false, ColumnDataType = "decimal(10,4)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [FsColumn(IsNullable = false, ColumnDataType = "decimal(10,4)")]
    public decimal Value { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [FsColumn(9999)]
    public string Remark { get; set; }

}
