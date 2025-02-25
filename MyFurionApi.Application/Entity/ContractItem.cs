﻿namespace MyFurionApi.Application.Entity;

/// <summary>
/// 合同明细
/// </summary>
[FsTable()]
public class ContractItem : BaseEntity
{
    /// <summary>
    /// 所属合同Id
    /// </summary>
    [FsColumn("所属合同Id", false), ForeignKeyTag]
    public int ContractId { get; set; }

    /// <summary>
    /// 明细名称
    /// </summary>
    [FsColumn("明细名称", false, 500)]
    public string Name { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [FsColumn("单价", false, ColumnDataType = DBColumnDataType.Money)]
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    [FsColumn("数量", false, ColumnDataType = "decimal(10,4)")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    [FsColumn("金额", false, ColumnDataType = DBColumnDataType.Money)]
    public decimal Value { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [FsColumn("备注", true, 9999)]
    public string Remark { get; set; }

}
