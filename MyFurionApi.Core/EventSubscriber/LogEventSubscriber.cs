using Furion.DependencyInjection;
using Furion.EventBus;
using Microsoft.Extensions.Logging;

namespace MyFurionApi.Core;

/// <summary>
/// 日志事件订阅
/// </summary>
public class LogEventSubscriber : IEventSubscriber, ISingleton
{
    private readonly ILogger<LogEventSubscriber> _logger;

    public IServiceProvider Services { get; }

    public LogEventSubscriber(ILogger<LogEventSubscriber> logger, IServiceProvider services)
    {
        _logger = logger;
        Services = services;
    }

    /// <summary>
    /// 订阅
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Create:OpLog")]
    public Task CreateOpLog(EventHandlerExecutingContext context)
    {
        //using var scope = Services.CreateScope();
        //var _repository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogOp>>();
        //var log = (SysLogOp)context.Source.Payload;
        //await _repository.InsertAsync(log);

        _logger.LogInformation("日志事件订阅收到消息：" + context.Source.Payload.ToString());

        return Task.CompletedTask;
    }


}
