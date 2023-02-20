namespace MyFurionApi.Application.Controller;

/// <summary>
/// 图形验证码
/// </summary>
[AllowAnonymous]
[Route("api/[controller]")]
public class ImgCaptchaController : BaseApiController
{
    private readonly ILogger<ImgCaptchaController> _logger;
    private readonly IImgCaptchaService _imgCaptchaService;

    public ImgCaptchaController(ILogger<ImgCaptchaController> logger, IImgCaptchaService imgCaptchaService)
    {
        _logger = logger;
        _imgCaptchaService = imgCaptchaService;
    }

    /// <summary>
    /// 获取操作标识
    /// </summary>
    /// <remarks>返回标识长度：32</remarks>
    /// <returns></returns>
    [HttpGet("get/identification")]
    public string GetActionIdentification()
    {
        return Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// 生成图形验证码
    /// </summary>
    /// <param name="tag">操作标识（通过【获取操作标识】接口获得）</param>
    /// <remarks>返回是文件流，直接img标签src复制</remarks>
    /// <returns></returns>
    [HttpGet("get")]
    public async Task<IActionResult> Generate(string tag)
    {
        if (tag.IsNull()) throw Oops.Bah("tag不能为空");
        var ms = await _imgCaptchaService.GenerateAsync(tag);
        
        return new FileContentResult(ms.ToArray(), @"image/png");
    }

    /// <summary>
    /// 校验图形验证码
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("check")]
    public async Task<string> Check(string tag, string code)
    {
        await _imgCaptchaService.ValidateCodeAsync(tag, code);
        return "校验通过";
    }

}
