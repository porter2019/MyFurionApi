namespace MyFurionApi.Application.Dto;

/// <summary>
/// 查询单条实体所需条件，自动构建where
/// </summary>
public class ContractSingleInput : BaseSingleQueryModel
{
    /// <summary>
    /// 名称
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Code { get; set; }
}
