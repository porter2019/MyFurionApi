using Furion.HttpRemote;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 调用远程接口
/// </summary>
public class RemoteController : BaseApiController
{
    private readonly IHttpRemoteService _httpRemoteService;

    public RemoteController(IHttpRemoteService httpRemoteService)
    {
        _httpRemoteService = httpRemoteService;
    }

    /// <summary>
    /// Get请求
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public Task<string> Get()
    {
        //更多用法 https://furion.net/docs/http-agent
        return _httpRemoteService.GetAsStringAsync("http://www.baidu.com");
    }

}
