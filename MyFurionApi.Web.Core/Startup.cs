using Furion;
using Furion.VirtualFileServer;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MyFurionApi.Core;
using System;
using System.Text.Encodings.Web;

namespace MyFurionApi.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        //配置文件使用实体
        services.AddConfigurableOptions<CacheOptions>();

        #region 上传文件大小限制
        long maxRequestBodySize = Convert.ToInt64(App.Configuration["MaxRequestBodySize"]);
        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxRequestBodySize;
        });
        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = maxRequestBodySize;
        });

        services.Configure<FormOptions>(o =>
        {
            o.MultipartBodyLengthLimit = maxRequestBodySize;
        });
        #endregion

        //services.AddConsoleFormatter();

        //日志输出到文件
        services.AddFileLogging("logs/{0:yyyy}-{0:MM}-{0:dd}.log", options =>
        {
            options.FileNameRule = fileName =>
            {
                return string.Format(fileName, DateTime.UtcNow);
            };
        });

        //全局捕捉接口控制器详细日志信息
        services.AddMonitorLogging();//默认读取 Logging:Monitor 下配置

        //处理使用nginx代理后IP获取不正确的问题
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
        });

        //SqlSugar
        services.AddSqlsugarSetup();
        //JWT
        services.AddJwt<JwtHandler>(enableGlobalAuthorize: true); //启用全局登录验证 //services.AddJwt<JwtHandler>(); //
        //跨域
        services.AddCorsAccessor();
        //远程请求
        services.AddRemoteRequest();
        //虚拟文件系统
        services.AddVirtualFileServer();

        services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(MyExceptionFilter));
                    options.Filters.Add(typeof(MyActionFilter));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.DateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.DateTimeNullableConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.IntToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.DoubleToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.DecimalToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConfig.StringJsonConverter());
                })
                .AddInjectWithUnifyResult();

        //事件订阅
        //services.AddEventBus(builder =>
        //{
        //    builder.AddSubscriber<LogEventSubscriber>();
        //});
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //处理使用nginx代理后IP获取不正确的问题 放到最前面
        app.UseForwardedHeaders();

        // 添加规范化结果状态码
        app.UseUnifyResultStatusCodes();

        //app.UseHttpLogging();

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = FS.GetFileExtensionContentTypeProvider(),//拓展MIME支持
            //FileProvider = new PhysicalFileProvider(staticDirectory) //如果wwwroot不在网站默认的根目录下，则这里指定
        });

        app.UseCorsAccessor();

        app.UseAuthentication();
        app.UseAuthorization();

        //使用 Knife4jUI 替换Swagger
        var routePrefix = "doc";
        app.UseKnife4UI(options =>
        {
            options.RoutePrefix = routePrefix;  // 配置 Knife4UI 路由地址
            foreach (var groupInfo in Furion.SpecificationDocument.SpecificationDocumentBuilder.GetOpenApiGroups())
            {
                options.SwaggerEndpoint("/" + groupInfo.RouteTemplate, groupInfo.Title);
            }
        });
        app.UseInject(routePrefix); //app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
