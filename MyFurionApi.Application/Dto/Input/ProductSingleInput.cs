namespace MyFurionApi.Application.Dto;

/// <summary>
/// 构建where
/// </summary>
public class ProductSingleInput : BaseSingleQueryModel
{
    /// <summary>
    /// 名称
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }
}
