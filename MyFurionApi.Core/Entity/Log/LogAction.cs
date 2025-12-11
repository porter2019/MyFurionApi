using System.ComponentModel;

namespace MyFurionApi.Core;

/// <summary>
/// 操作日志
/// </summary>
[FsTable()]
public class LogAction : BaseEntityStandard
{
    /// <summary>
    /// 客户端类型
    /// </summary>
    [FsColumn()]
    public ClientFromType ClientType { get; set; } = ClientFromType.未知;

    [FsColumn(true)]
    public string ClientTypeName => ClientType.GetEnumDescription();

    /// <summary>
    /// 操作类型
    /// </summary>
    [FsColumn()]
    public LogActionType Type { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    [FsColumn(true)]
    public string TypeName => Type.GetEnumDescription();

    /// <summary>
    /// 操作位置
    /// </summary>
    [FsColumn(200)]
    public string Local { get; set; }

    /// <summary>
    /// 操作内容
    /// </summary>
    [FsColumn(9999)]
    public string Content { get; set; }

    #region 额外数据

    /// <summary>
    /// 拼接-功能名称
    /// <code>例如：采购订单</code>
    /// </summary>
    [FsColumn(true)]
    public string ExtraHandler { get; set; }

    /// <summary>
    /// 拼接-业务编号等
    /// <code>例如：订单编号：PS251030003</code>
    /// </summary>
    [FsColumn(true)]
    public string ExtraInfo { get; set; }

    #endregion

}

/// <summary>
/// 操作类型
/// </summary>
public enum LogActionType
{
    [Description("新增")]
    新增 = 1,
    [Description("修改")]
    修改,
    [Description("删除")]
    删除,
}
