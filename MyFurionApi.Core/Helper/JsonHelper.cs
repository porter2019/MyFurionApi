using System.Text.Json.Serialization;
using System.Text.Json;

namespace MyFurionApi.Core;

/// <summary>
/// Json序列号
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// 序列化对象为json字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Serialize(object value)
    {
        if (value == null) return "";
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNamingPolicy = null,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };
        //options.Converters.Add(item: new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
        options.Converters.Add(new SystemTextJsonConfig.DateTimeConverter());
        options.Converters.Add(new SystemTextJsonConfig.DateTimeNullableConverter());
        options.Converters.Add(new SystemTextJsonConfig.IntToStringConverter());
        options.Converters.Add(new SystemTextJsonConfig.DoubleToStringConverter());
        options.Converters.Add(new SystemTextJsonConfig.DecimalToStringConverter());
        options.Converters.Add(new SystemTextJsonConfig.BoolJsonConverter());
        options.Converters.Add(new SystemTextJsonConfig.StringJsonConverter());
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// 序列化json字符串为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string json)
    {
        if (json.IsNull()) return default;

        try
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = null,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            //options.Converters.Add(item: new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));
            options.Converters.Add(new SystemTextJsonConfig.DateTimeConverter());
            options.Converters.Add(new SystemTextJsonConfig.DateTimeNullableConverter());
            options.Converters.Add(new SystemTextJsonConfig.IntToStringConverter());
            options.Converters.Add(new SystemTextJsonConfig.DoubleToStringConverter());
            options.Converters.Add(new SystemTextJsonConfig.DecimalToStringConverter());
            options.Converters.Add(new SystemTextJsonConfig.BoolJsonConverter());
            options.Converters.Add(new SystemTextJsonConfig.StringJsonConverter());
            return JsonSerializer.Deserialize<T>(json, options);
        }
        catch
        {
            return default;
        }
    }
}
