namespace MyFurionApi.Application.Controller;

/// <summary>
/// 健康检查
/// </summary>
[Route("api/[controller]")]
[ApiDescriptionSettings(false)]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 检查
    /// </summary>
    /// <returns>/api/health/check</returns>
    [HttpGet, Route("check"), AllowAnonymous, SwaggerIgnore]
    public string Check()
    {
        //SwaggerIgnore文档中隐藏此接口
        return "OK";
    }

}
