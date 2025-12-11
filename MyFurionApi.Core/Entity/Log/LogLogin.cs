namespace MyFurionApi.Core;

/// <summary>
/// 用户登录日志
/// </summary>
[FsTable()]
public class LogLogin : BaseEntityStandard
{
    /// <summary>
    /// 客户端类型
    /// </summary>
    [FsColumn()]
    public ClientFromType ClientType { get; set; } = ClientFromType.未知;

    [FsColumn(true)]
    public string ClientTypeName => ClientType.GetEnumDescription();

    /// <summary>
    /// 用户Id
    /// </summary>
    [FsColumn()]
    public int UserId { get; set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    [FsColumn(30)]
    public string UserName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [FsColumn(15)]
    public string CellPhone { get; set; }

    /// <summary>
    /// 登录平台
    /// </summary>
    [FsColumn(20)]
    public string Platform { get; set; }

    /// <summary>
    /// 登录IP
    /// </summary>
    [FsColumn(30)]
    public string IP { get; set; }

    /// <summary>
    /// 登录UA
    /// </summary>
    [FsColumn(500)]
    public string UserAgent { get; set; }

    ///// <summary>
    ///// 备注
    ///// </summary>
    //[FsColumn("备注", true, 200)]
    //public string Remark { get; set; }

}
