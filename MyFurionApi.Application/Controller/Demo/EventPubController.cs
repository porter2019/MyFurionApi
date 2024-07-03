using Furion.EventBus;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 事件发布测试
/// </summary>
public class EventPubController : BaseApiController
{
    private readonly IEventPublisher _eventPublisher;

    public EventPubController(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string> Pub(string msg)
    {
        await _eventPublisher.PublishAsync("Create:OpLog", msg);
        return "消息已发布";
    }

}
