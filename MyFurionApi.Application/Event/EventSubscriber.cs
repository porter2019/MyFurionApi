using Furion.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace MyFurionApi.Application.Event;

/// <summary>
/// 事件订阅器
/// <code>追加继承ISingleton接口，就不用在Startup中一个个注册了</code>
/// </summary>
public class EventSubscriber : IEventSubscriber
{
    private readonly ILogger<EventSubscriber> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public EventSubscriber(ILogger<EventSubscriber> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }


    /// <summary>
    /// 料仓当前容量事件
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe("Silo:CurValue")]
    public async Task SiloCurrValue(EventHandlerExecutingContext context)
    {
        //var payload = (EventSiloCurPayload)context.Source.Payload;
        //if (payload == null) return;
        var payload = (string)context.Source.Payload;
        if (payload.IsNull()) return;

        using var scope = _scopeFactory.CreateScope();
        var _publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        var _userRepository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysUser>>();
        await _userRepository.FirstOrDefaultAsync(x => x.Id == 1);
    }
}
