using Furion.Logging.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Yitter.IdGenerator;

namespace MyFurionApi.Application;

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
        //配置短Id
        var options = new IdGeneratorOptions(App.Configuration.Get<ushort>("AppSettings:YitterShortIdWorkId"));
        YitIdHelper.SetIdGenerator(options);

        //后台服务
        //services.AddHostedService<ReceiveWorker>();

        //事件订阅
        services.AddEventBus(builder =>
        {
            builder.UnobservedTaskExceptionHandler = (obj, args) =>
            {
                var msg = $"【订阅事件出现异常】obj:{obj},异常消息:{args.Exception.Message}";
                msg.LogError(args.Exception);
            };
        });
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
