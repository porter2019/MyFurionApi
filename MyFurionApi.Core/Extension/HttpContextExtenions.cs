using Microsoft.AspNetCore.Http;
using System.Net;

namespace MyFurionApi.Core;

/// <summary>
/// HttpContext扩展
/// </summary>
public static class HttpContextExtenions
{
    /// <summary>
    /// 获取客户端真实 IP 地址
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string GetClientIpAddress(this HttpContext context)
    {
        if (context == null)
            return string.Empty;

        // 1. 尝试从 X-Forwarded-For 获取
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            var ips = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(ip => ip.Trim())
                                  .Where(ip => !string.IsNullOrEmpty(ip))
                                  .FirstOrDefault();

            if (TryParseIP(ips, out var forwardedIp))
                return forwardedIp.ToString();
        }

        // 2. 尝试从 X-Real-IP 获取
        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp) && TryParseIP(xRealIp, out var realIp))
        {
            return realIp.ToString();
        }

        // 3. 尝试从 CF-Connecting-IP (Cloudflare) 获取
        var cfIp = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(cfIp) && TryParseIP(cfIp, out var cloudflareIp))
        {
            return cloudflareIp.ToString();
        }

        // 4. 返回直接连接的 IP
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            // 如果是 IPv6 映射的 IPv4，转换为 IPv4
            if (remoteIp.IsIPv4MappedToIPv6)
                return remoteIp.MapToIPv4().ToString();

            return remoteIp.ToString();
        }

        return string.Empty;
    }

    private static bool TryParseIP(string ipString, out IPAddress ipAddress)
    {
        ipAddress = default; // 或者 null! 如果确定不会为空
        if (string.IsNullOrEmpty(ipString))
            return false;

        return IPAddress.TryParse(ipString, out ipAddress);
    }
}
