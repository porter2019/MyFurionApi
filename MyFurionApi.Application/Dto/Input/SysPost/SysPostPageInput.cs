namespace MyFurionApi.Application.Dto;

/// <summary>
/// 岗位管理分页查询
/// </summary>
public class SysPostPageInput : BasePageQueryModel<SysPost>
{
    /// <summary>
    /// Name
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string Name { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [PageQuery(PageQueryOperatorType.Equal)]
    public bool? Status { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }
}

