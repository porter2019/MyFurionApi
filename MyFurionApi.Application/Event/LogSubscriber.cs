using Microsoft.Extensions.DependencyInjection;

namespace MyFurionApi.Application.Event;

/// <summary>
/// 日志订阅
/// </summary>
public class LogSubscriber : IEventSubscriber, ISingleton
{
    private readonly IServiceScopeFactory _scopeFactory;

    public LogSubscriber(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// 添加登录日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Log:Login:Add")]
    public async Task LogLoginAdd(EventHandlerExecutingContext context)
    {
        var payload = (LogLogin)context.Source.Payload;
        if (payload == null) return;

        using var scope = _scopeFactory.CreateScope();

        var logRepository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<LogLogin>>();
        await logRepository.InsertAsync(payload);
    }

    /// <summary>
    /// 添加操作日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Log:Action:Add")]
    public async Task LogActionAdd(EventHandlerExecutingContext context)
    {
        var payload = (LogAction)context.Source.Payload;
        if (payload == null) return;

        using var scope = _scopeFactory.CreateScope();

        var logRepository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<LogAction>>();

        await logRepository.InsertAsync(payload);
    }

}
