namespace MyFurionApi.Application.Dto;

/// <summary>
/// 操作日志分页查询
/// </summary>
public class LogActionPageInput : BasePageQueryModel<LogAction>
{
    /// <summary>
    /// 类型
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterZero)]
    public int Type { get; set; }

    /// <summary>
    /// 客户端类型
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterMinus)]
    public int? ClientType { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string CreatedUserName { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }
}

