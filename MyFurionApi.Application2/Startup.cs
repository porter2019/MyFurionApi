using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace MyFurionApi.Application2
{
    /// <summary>
    /// 额外需要使用的服务
    /// </summary>
    public class Startup : AppStartup
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddRemoteRequest();
            //services.AddEventBus();
            //services.AddEventBus(builder =>
            //{
            //    builder.AddSubscriber<LogEventSubscriber>();
            //});
        }

        /// <summary>
        /// 使用
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}
