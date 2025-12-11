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
}
