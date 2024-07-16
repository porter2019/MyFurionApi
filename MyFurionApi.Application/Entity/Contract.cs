namespace MyFurionApi.Application.Entity;

/// <summary>
/// 合同
/// </summary>
[SugarTable(null)]
public class Contract : BaseEntityStandard
{
    /// <summary>
    /// 合同编号
    /// </summary>
    [FsColumn("合同编号", false, 200)]
    public string Code { get; set; }

    /// <summary>
    /// 合同名称
    /// </summary>
    [FsColumn("合同名称", false, 500)]
    public string Name { get; set; }

    /// <summary>
    /// 合同类型
    /// </summary>
    [FsColumn("合同类型", false)]
    public ContractTypeEnum CType { get; set; } = ContractTypeEnum.Buyer;

    /// <summary>
    /// 合同类型名称
    /// </summary>
    [FsColumn(true)]
    public string CTypeName => CType.GetEnumDescription();

    /// <summary>
    /// 签订时间
    /// </summary>
    [FsColumn("签订时间")]
    public DateTime? SignDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 合同金额
    /// </summary>
    [FsColumn("合同金额", false, ColumnDataType = DBColumnDataType.Money)]
    public decimal Value { get; set; }

    /// <summary>
    /// 摘要
    /// </summary>
    [FsColumn("摘要", true, 2000)]
    public string Summary { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    [FsColumn("审批状态", false)]
    public ContractFlagEnum Flag { get; set; } = ContractFlagEnum.草稿;

    /// <summary>
    /// 审批状态名称
    /// </summary>
    [FsColumn(true)]
    public string FlagName => Flag.GetEnumDescription();


    #region 关联数据

    /// <summary>
    /// 明细
    /// </summary>
    [FsColumn(true)]
    public List<ContractItem> ItemList { get; set; }

    /// <summary>
    /// 附件
    /// </summary>
    [FsColumn(true)]
    public List<CommonAttach> AttachList { get; set; }

    #endregion

}


/// <summary>
/// 合同类型
/// </summary>
public enum ContractTypeEnum
{
    [Description("采购合同")]
    Buyer = 1,

    [Description("销售合同")]
    Seller
}


/// <summary>
/// 合同状态
/// </summary>
public enum ContractFlagEnum
{
    [Description("草稿")]
    草稿 = 1,

    [Description("审批中")]
    审批中,

    [Description("审批通过")]
    审批通过,

    [Description("审批不通过")]
    审批不通过
}