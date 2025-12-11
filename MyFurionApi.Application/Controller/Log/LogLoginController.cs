namespace MyFurionApi.Application.Controller;

/// <summary>
/// 登录日志
/// </summary>
[PermissionHandler("日志记录", "登录日志", "LogLogin", 0)]
public class LogLoginController : BaseApiController
{
    private readonly ILogger<LogLoginController> _logger;
    private readonly SqlSugarRepository<LogLogin> _logLoginRepository;

    public LogLoginController(ILogger<LogLoginController> logger,
        SqlSugarRepository<LogLogin> logLoginRepository)
    {
        _logger = logger;
        _logLoginRepository = logLoginRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist"), Permission("查看", "show")]
    public async Task<SqlSugarPagedList<LogLogin>> GetPageList(LogLoginPageInput req)
    {
        UnifyContext.Fill(new
        {
            Enums = new List<dynamic> {
                new { Name = "ClientType", Options = typeof(ClientFromType).GetEnumOptions() }
            }
        });
        var data = await _logLoginRepository.ToPageListAsync(req);
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
        await _logLoginRepository.DeleteAsync(ids.SplitWithComma().ConvertIntList());
        return "删除成功";
    }
}

