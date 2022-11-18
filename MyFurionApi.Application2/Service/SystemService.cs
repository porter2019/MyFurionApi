namespace MyFurionApi.Application2
{
    /// <summary>
    /// 演示系统服务
    /// </summary>
    public class SystemService : ISystemService, ITransient
    {
        private readonly ILogger<SystemService> _logger;

        public SystemService(ILogger<SystemService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Say
        /// </summary>
        /// <returns></returns>
        public string Hello()
        {
            return "Hello World";
        }

    }
}
