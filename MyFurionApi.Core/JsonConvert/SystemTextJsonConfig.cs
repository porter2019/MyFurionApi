using System.Buffers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyFurionApi.Core;

/// <summary>
/// SystemTextJson序列化配置
/// </summary>
public class SystemTextJsonConfig
{
    /// <summary>
    /// 时间格式化
    /// </summary>
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            //时分秒为0则返回短日期
            //writer.WriteStringValue((value.Hour == 0 && value.Minute == 0 && value.Millisecond == 0) ? value.ToString("yyyy-MM-dd") : value.ToString("yyyy-MM-dd HH:mm:ss"));
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    /// <summary>
    /// 可空时间类型格式化
    /// </summary>
    public class DateTimeNullableConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return string.IsNullOrEmpty(reader.GetString()) ? default(DateTime?) : DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }

    /// <summary>
    /// 解决数值字符串不能隐式转换为数值类型的问题(隐式转换会出现精度缺失, 但依旧会转换成功最终导致数据计算或者数据落库等安全隐患, 是个潜在的问题)
    /// </summary>
    public class IntToStringConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (System.Buffers.Text.Utf8Parser.TryParse(span, out int number, out int bytesConsumed) && span.Length == bytesConsumed)
                {
                    return number;
                }

                if (Int32.TryParse(reader.GetString(), out number))
                {
                    return number;
                }
            }
            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    /// <summary>
    /// 双精度转换
    /// </summary>
    public class DoubleToStringConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (System.Buffers.Text.Utf8Parser.TryParse(span, out double number, out int bytesConsumed) && span.Length == bytesConsumed)
            {
                return number;
            }

            if (Double.TryParse(reader.GetString(), out number))
            {
                return number;
            }

            return reader.GetDouble();
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);//value.ObjToThousString()
        }
    }

    /// <summary>
    /// Decimal
    /// </summary>
    public class DecimalToStringConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
            if (System.Buffers.Text.Utf8Parser.TryParse(span, out decimal number, out int bytesConsumed) && span.Length == bytesConsumed)
            {
                return number;
            }

            if (Decimal.TryParse(reader.GetString(), out number))
            {
                return number;
            }

            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    /// <summary>
    /// 布尔类型处理
    /// </summary>
    public class BoolJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                return reader.GetBoolean();

            return bool.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    /// <summary>
    /// 将string类型的Null值转成""
    /// </summary>
    public class StringJsonConverter : JsonConverter<string>
    {
        public override bool HandleNull => true;

        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString();
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(string.IsNullOrWhiteSpace(value) ? string.Empty : value);
        }
    }
}
