namespace MyFurionApi.Application.Dto;

/// <summary>
/// 查询列表所需数据
/// </summary>
public class ContractListInput : BaseListQueryModel
{
    /// <summary>
    /// 名称like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }
}
