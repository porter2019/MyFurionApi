namespace MyFurionApi.Application2.ApiController
{
    /// <summary>
    /// 演示模块
    /// </summary>
    public class DemoController : BaseApiController
    {
        private readonly ILogger<DemoController> _logger;

        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 演示模块-AAA
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string AAA()
        {
            return "这里是App2模块下的方法";
        }

    }
}
