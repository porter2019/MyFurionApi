namespace MyFurionApi.Application.Dto;

/// <summary>
/// 系统用户分页查询
/// </summary>
public class SysUserPageInput : BasePageQueryModel<SysUserOMInfo>
{
    /// <summary>
    /// OrgId
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterZero)]
    public int? OrgId { get; set; }

    /// <summary>
    /// CellPhone like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string CellPhone { get; set; }

    /// <summary>
    /// OrgName like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string OrgName { get; set; }

    /// <summary>
    /// 组名 like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string RoleNames { get; set; }

    /// <summary>
    /// 岗位 like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string PostNames { get; set; }

    /// <summary>
    /// UserName like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string UserName { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }
}