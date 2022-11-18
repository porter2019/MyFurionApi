namespace MyFurionApi.Application2;

/// <summary>
/// 接口控制器基类
/// </summary>
[Route("api/second/[controller]")]
[ApiDescriptionSettings("App2")]
public class BaseApiController : IDynamicApiController
{
    public BaseApiController()
    {

    }
}
