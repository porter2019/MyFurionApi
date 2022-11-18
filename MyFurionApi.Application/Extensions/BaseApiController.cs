namespace MyFurionApi.Application;

[Route("api/[controller]")]
[ApiDescriptionSettings("Default")]
public class BaseApiController : IDynamicApiController
{
    public BaseApiController()
    {

    }
}
