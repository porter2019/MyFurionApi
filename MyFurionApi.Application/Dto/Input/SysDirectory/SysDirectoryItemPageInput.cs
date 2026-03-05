namespace MyFurionApi.Application.Dto;

/// <summary>
/// 字典管理分页查询
/// </summary>
public class SysDirectoryItemPageInput : BasePageQueryModel<SysDirectoryItemFullInfo>
{
    /// <summary>
    /// Name
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string Name { get; set; }

    /// <summary>
    /// CategoryId
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterZero)]
    public int CategoryId { get; set; }

    /// <summary>
    /// CategoryName
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string CategoryName { get; set; }

    /// <summary>
    /// CategoryCode
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string CategoryCode { get; set; }

    /// <summary>
    /// CategoryCode In查询，多个,分割
    /// </summary>
    [PageQuery(PageQueryOperatorType.StringIn, ColumnName = "CategoryCode")]
    public string CategoryCodeIn { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [PageQuery(PageQueryOperatorType.Equal)]
    public bool? Status { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterZero)]
    public int? TenantId { get; set; }

    /// <summary>
    /// 企业名称
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string TenantName { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }
}

