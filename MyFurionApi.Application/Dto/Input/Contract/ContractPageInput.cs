namespace MyFurionApi.Application.Dto;

/// <summary>
/// 产品分页所需数据
/// </summary>
public class ContractPageInput : BasePageQueryModel<Contract>
{
    /// <summary>
    /// code like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Code { get; set; }

    /// <summary>
    /// 名称like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }
}
