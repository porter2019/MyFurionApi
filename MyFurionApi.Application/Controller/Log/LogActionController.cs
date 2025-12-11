namespace MyFurionApi.Application.Controller;

/// <summary>
/// 操作日志
/// </summary>
[PermissionHandler("日志记录", "操作日志", "LogAction", 10)]
public class LogActionController : BaseApiController
{
    private readonly ILogger<LogActionController> _logger;
    private readonly SqlSugarRepository<LogAction> _logActionRepository;

    public LogActionController(ILogger<LogActionController> logger,
        SqlSugarRepository<LogAction> logActionRepository)
    {
        _logger = logger;
        _logActionRepository = logActionRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist"), Permission("查看", "show")]
    public async Task<SqlSugarPagedList<LogAction>> GetPageList(LogActionPageInput req)
    {
        UnifyContext.Fill(new
        {
            Enums = new List<dynamic> {
                new { Name = "Type", Options = typeof(LogActionType).GetEnumOptions() },
                new { Name = "ClientType", Options = typeof(ClientFromType).GetEnumOptions() }
            }
        });

        var data = await _logActionRepository.ToPageListAsync(req);
        return data;
    }


    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete, UnitOfWork, Permission("删除", "delete")]
    public async Task<string> Delete(string ids)
    {
        await _logActionRepository.DeleteAsync(ids.SplitWithComma().ConvertIntList());
        return "删除成功";
    }
}

