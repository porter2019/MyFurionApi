using Furion.RemoteRequest.Extensions;

namespace MyFurionApi.Application.Controller
{
    /// <summary>
    /// 调用远程接口
    /// </summary>
    public class RemoteController : BaseApiController
    {
        public RemoteController()
        {

        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<string> Get()
        {
            //更多用法 https://furion.baiqian.ltd/docs/http
            return "http://www.baidu.com".GetAsStringAsync();
        }

    }
}
