namespace MyFurionApi.Application.Entity;

/// <summary>
/// 合同
/// </summary>
[FsTable()]
public class Contract : BaseEntityStandard
{
    /// <summary>
    /// 合同编号
    /// </summary>
    [FsColumn(200)]
    public string Code { get; set; }

    /// <summary>
    /// 合同名称
    /// </summary>
    [FsColumn(500)]
    public string Name { get; set; }

    /// <summary>
    /// 合同类型
    /// </summary>
    [FsColumn(IsNullable = false)]
    public ContractTypeEnum CType { get; set; } = ContractTypeEnum.Buyer;

    /// <summary>
    /// 合同类型名称
    /// </summary>
    [FsColumn(true)]
    public string CTypeName => CType.GetEnumDescription();

    /// <summary>
    /// 签订时间
    /// </summary>
    [FsColumn()]
    public DateTime? SignDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 合同金额
    /// </summary>
    [FsColumn(IsNullable = false, ColumnDataType = DBColumnDataType.Money)]
    public decimal Value { get; set; }

    /// <summary>
    /// 省份
    /// </summary>
    [FsColumn(30)]
    public string RegionProvince { get; set; }

    /// <summary>
    /// 市
    /// </summary>
    [FsColumn(30)]
    public string RegionCity { get; set; }

    /// <summary>
    /// 区
    /// </summary>
    [FsColumn(30)]
    public string RegionDistrict { get; set; }

    /// <summary>
    /// 省市区聚合
    /// </summary>
    [FsColumn(200)]
    public string RegionFull { get; set; }

    /// <summary>
    /// 详细地址
    /// </summary>
    [FsColumn(50)]
    public string RegionAddress { get; set; }

    /// <summary>
    /// 审批状态
    /// </summary>
    [FsColumn(IsNullable = false)]
    public ContractFlagEnum Flag { get; set; } = ContractFlagEnum.草稿;

    /// <summary>
    /// 审批状态名称
    /// </summary>
    [FsColumn(true)]
    public string FlagName => Flag.GetEnumDescription();

    /// <summary>
    /// 状态
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool Status { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [FsColumn(200)]
    public string Remark { get; set; }

    /// <summary>
    /// 附件
    /// </summary>
    [FsColumn(IsJson = true)]
    public List<CommonAttach> AttachList { get; set; } = [];

    #region 关联数据

    /// <summary>
    /// 明细
    /// </summary>
    [FsColumn(true)]
    public List<ContractItem> ItemList { get; set; } = [];

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