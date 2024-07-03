namespace MyFurionApi.Application.Dto;

/// <summary>
/// 更新合同
/// </summary>
public class ContractModifyInput : BaseFormPostModel
{
    /// <summary>
    /// 合同名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 合同类型
    /// </summary>
    public ContractTypeEnum CType { get; set; }

    /// <summary>
    /// 签订时间
    /// </summary>
    public DateTime? SignDate { get; set; }

    /// <summary>
    /// 合同金额
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// 明细
    /// </summary>
    public List<ContractItemModifyInput> ItemList { get; set; }

    /// <summary>
    /// 附件
    /// </summary>
    public List<AttachModifyInput> AttachList { get; set; }

}

/// <summary>
/// 修改合同明细所需
/// </summary>
public class ContractItemModifyInput
{
    /// <summary>
    /// 明细名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 金额
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}
