using Furion.ConfigurableOptions;

namespace MyFurionApi.Core;

/// <summary>
/// 缓存配置
/// </summary>
public class CacheOptions : IConfigurableOptions
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public CacheType CacheType { get; set; }

    /// <summary>
    /// Redis配置
    /// </summary>
    public string RedisConnectionString { get; set; }
}
