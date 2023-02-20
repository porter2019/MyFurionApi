namespace MyFurionApi.Application.Controller;

/// <summary>
/// 缓存
/// </summary>
[AllowAnonymous]
public class CacheController : BaseApiController
{
    private readonly ILogger<CacheController> _logger;
    private readonly ICacheMyService _cacheService;
    private readonly CacheOptions _cacheOptions;

    public CacheController(ILogger<CacheController> logger, IOptions<CacheOptions> cacheOptions, Func<string, ISingleton, object> resolveNamed)
    {
        _logger = logger;
        _cacheOptions = cacheOptions.Value;// 配置文件
        // 因为都注入了MemoryCache和RedisCache，所以这里根据配置文件来取具体使用哪个服务
        _cacheService = resolveNamed(_cacheOptions.CacheType.ToString(), default) as ICacheMyService;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public bool Add(string key, string value)
    {
        //TimeSpan.FromMinutes(1);//1分钟后
        return _cacheService.Set(key, value, TimeSpan.FromMinutes(1));
    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpGet]
    public string Get(string key)
    {
        return _cacheService.Get(key);
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [HttpGet]
    public long Del(string key)
    {
        return _cacheService.Del(key);
    }

}
