namespace MyFurionApi.Application2.Dto;

/// <summary>
/// 产品列表所需数据
/// </summary>
public class TestListInput : BaseListQueryModel
{
    /// <summary>
    /// 名称like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }
}
