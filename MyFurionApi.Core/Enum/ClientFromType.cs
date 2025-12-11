using System.ComponentModel;

namespace MyFurionApi.Core;

/// <summary>
/// 前端客户端类型
/// </summary>
public enum ClientFromType
{
    [Description("未知")]
    未知,

    [Description("后台")]
    后台,

    [Description("小程序")]
    小程序,
}
