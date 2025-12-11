namespace MyFurionApi.Application.Dto;

/// <summary>
/// 登录日志分页查询
/// </summary>
public class LogLoginPageInput : BasePageQueryModel<LogLogin>
{

    /// <summary>
    /// 客户端类型
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterMinus)]
    public int? ClientType { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string UserName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [PageQuery(PageQueryOperatorType.CharIndex)]
    public string CellPhone { get; set; }

    /// <summary>
    /// UserId
    /// </summary>
    [PageQuery(PageQueryOperatorType.IntEqualWhenGreaterZero)]
    public int UserId { get; set; }

    /// <summary>
    /// 添加时间
    /// </summary>
    [PageQuery(PageQueryOperatorType.BetweenDate)]
    public string CreatedTime { get; set; }
}

