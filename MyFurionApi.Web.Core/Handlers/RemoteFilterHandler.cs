using Furion.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFurionApi.Web.Core;

/// <summary>
/// 远程请求拦截器
/// </summary>
public class RemoteFilterHandler : DelegatingHandler, ITransient
{
    private readonly ILogger<RemoteFilterHandler> _logger;

    public RemoteFilterHandler(ILogger<RemoteFilterHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 自定义逻辑，比如判断是否携带 X-API-KEY 头，也可以动态添加 Token 等等
        //if (!request.Headers.Contains("X-API-KEY"))
        //{
        //    return new HttpResponseMessage(HttpStatusCode.BadRequest)
        //    {
        //        Content = new StringContent(
        //            "The API key header X-API-KEY is required.")
        //    };
        //}
        var logTemplate = new StringBuilder();
        logTemplate.AppendLine($"【HTTP请求】Method：{request.Method}，URL：{request.RequestUri}，Headers：");
        foreach (var header in request.Headers)
        {
            foreach (var value in header.Value)
            {
                logTemplate.AppendLine($"{header.Key}: {value}");
            }
        }
        if (request.Content != null)
        {
            if (request.Content.Headers.ContentType.MediaType == "multipart/form-data")
                logTemplate.AppendLine("Body中含有文件");
            else //JSON文本
                logTemplate.AppendLine($"Body：{request.Content.ReadAsStringAsync().Result}");
        }

        _logger.LogDebug(logTemplate.ToString());
        var response = await base.SendAsync(request, cancellationToken);

        // 检查响应内容类型，只记录文本类型的内容
        var contentType = response.Content?.Headers?.ContentType?.MediaType;
        if (contentType != null && (contentType.StartsWith("text/") ||
                                   contentType.Contains("application/json") ||
                                   contentType.Contains("application/xml") ||
                                   contentType.Contains("application/javascript") ||
                                   contentType.Contains("application/x-www-form-urlencoded")))
        {
            var resultText = await response.Content.ReadAsStringAsync();
            _logger.LogDebug($"【HTTP响应】状态码：{response.StatusCode}，响应内容：{resultText}");
        }
        else
        {
            // 对于非文本类型（如PDF、图片等），只记录状态码和内容类型
            _logger.LogDebug($"【HTTP响应】状态码：{response.StatusCode}，内容类型：{contentType ?? "unknown"}，跳过内容记录");
        }

        return response;
    }

}
