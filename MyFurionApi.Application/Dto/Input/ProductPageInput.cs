namespace MyFurionApi.Application.Dto;

/// <summary>
/// 产品分页所需数据
/// </summary>
public class ProductPageInput : BasePageQueryModel<Product>
{
    /// <summary>
    /// 名称like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }
}
