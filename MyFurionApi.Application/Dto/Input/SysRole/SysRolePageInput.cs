namespace MyFurionApi.Application.Dto;
/// <summary>
/// 系统用户组分页查询
/// </summary>
public class SysRolePageInput : BasePageQueryModel<SysRole>
{
    /// <summary>
    /// like查询
    /// </summary>
    [PageQuery(PageQueryOperatorType.Like)]
    public string Name { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string UpdatedTime { get; set; }
}
