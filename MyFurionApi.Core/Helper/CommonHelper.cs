namespace MyFurionApi.Core;

/// <summary>
/// 通用帮助类
/// </summary>
public class CommonHelper
{
    /// <summary>
    /// 获取客户端类型
    /// </summary>
    /// <returns></returns>
    public static ClientFromType GetClientType()
    {
        var from = App.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == AppConst.RequstFromKey);
        if (from.Key.IsNull()) return ClientFromType.未知;
        switch (from.Value.ToString().ToLower())
        {
            case "om":
                return ClientFromType.后台;
            case "mp":
                return ClientFromType.小程序;
            default:
                return ClientFromType.未知;
        }
    }

    /// <summary>
    /// 将数字转换为中文数字（仅支持 0–99）
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static string ToChineseNumber(int number)
    {
        if (number < 0 || number > 99)
            throw new ArgumentOutOfRangeException(nameof(number), "仅支持 0–99");

        ReadOnlySpan<char> digits = ['零', '一', '二', '三', '四', '五', '六', '七', '八', '九'];

        if (number < 10)
            return digits[number].ToString();

        int tens = number / 10;
        int ones = number % 10;

        return (tens, ones) switch
        {
            (1, 0) => "十",
            (1, _) => $"十{digits[ones]}",
            (_, 0) => $"{digits[tens]}十",
            _ => $"{digits[tens]}十{digits[ones]}"
        };
    }

    /// <summary>
    /// 金额转中文
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static string MoneyToChinese(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("金额不能为负数");

        if (amount == 0)
            return "零元整";

        string[] digits = { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        string[] units = { "", "拾", "佰", "仟" };
        string[] bigUnits = { "", "万", "亿", "万亿" };
        string[] decimalUnits = { "角", "分" };

        long intPart = (long)Math.Truncate(amount);
        int decPart = (int)Math.Round((amount - intPart) * 100);

        var result = new System.Text.StringBuilder();

        // 处理整数部分
        if (intPart > 0)
        {
            string intStr = intPart.ToString();
            int len = intStr.Length;
            bool lastZero = false;

            for (int i = 0; i < len; i++)
            {
                int digit = intStr[i] - '0';
                int pos = len - i - 1;
                int bigUnitIndex = pos / 4;
                int unitIndex = pos % 4;

                if (digit == 0)
                {
                    lastZero = true;
                }
                else
                {
                    if (lastZero && result.Length > 0)
                        result.Append("零");
                    result.Append(digits[digit]);
                    result.Append(units[unitIndex]);
                    lastZero = false;
                }

                // 添加万、亿等大单位
                if (unitIndex == 0 && bigUnitIndex > 0)
                {
                    // 如果这一节全为零则不加大单位（但要保留亿）
                    bool sectionAllZero = true;
                    int sectionStart = i - (len - 1 - pos) % 4;
                    if (sectionStart < 0) sectionStart = 0;
                    for (int j = sectionStart; j <= i; j++)
                        if (intStr[j] != '0') { sectionAllZero = false; break; }

                    if (!sectionAllZero || bigUnitIndex == 2)
                        result.Append(bigUnits[bigUnitIndex]);
                }
            }

            result.Append("元");
        }

        // 处理小数部分
        if (decPart == 0)
        {
            result.Append("整");
        }
        else
        {
            int jiao = decPart / 10;
            int fen = decPart % 10;

            if (jiao > 0)
            {
                if (intPart == 0 && jiao == 0)
                    result.Append("零");
                result.Append(digits[jiao]);
                result.Append("角");
            }
            else if (intPart > 0)
            {
                result.Append("零");
            }

            if (fen > 0)
            {
                result.Append(digits[fen]);
                result.Append("分");
            }
        }

        return result.ToString();
    }
}
