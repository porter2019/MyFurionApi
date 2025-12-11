using Yitter.IdGenerator;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 系统
/// </summary>
public class SysController : BaseApiController
{
    private readonly ILogger<SysController> _logger;
    private readonly IConfiguration _config;

    public SysController(ILogger<SysController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    /// <summary>
    /// 获取系统配置文件中的值
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    public string GetConfigValue(string path)
    {
        if (path.IsNull()) return "path不能为空";
        return _config[path];
    }

    /// <summary>
    /// 获取IP地址
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public string GetIP()
    {
        var local = App.HttpContext.GetLocalIpAddressToIPv4();
        var remote = App.HttpContext.GetRemoteIpAddressToIPv4();
        var newIp = App.HttpContext.GetClientIpAddress();
        var rootPath = App.WebHostEnvironment.WebRootPath;
        var userAgent = App.HttpContext.Request.Headers["User-Agent"].ToString();
        return $"本机IP：{local}；客户端IP：{remote}；新方法获取的IP：{newIp}；WebRootPath：{rootPath}；UserAgent：{userAgent}；当前时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}；短Id：{YitIdHelper.NextId()}";
    }
}
