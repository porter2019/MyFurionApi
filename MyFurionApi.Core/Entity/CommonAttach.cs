namespace MyFurionApi.Core;

/// <summary>
/// 附件表
/// </summary>
[SqlSugar.SugarTable(null)]
public class CommonAttach : BaseEntity
{
    /// <summary>
    /// 类型
    /// </summary>
    public CommonAttachType AttachType { get; set; }

    /// <summary>
    /// 关联的数据id
    /// </summary>
    [FsColumn("数据Id"), ForeignKeyTag]
    public int RefId { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    [FsColumn("文件名称", true, 255)]
    public string FileName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [FsColumn("文件类型", true, 255)]
    public string FileType { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [FsColumn("文件大小")]
    public long FileSize { get; set; }

    /// <summary>
    /// 文件后缀名
    /// </summary>
    [FsColumn("文件后缀名", true, 50)]
    public string FileExt { get; set; }

    /// <summary>
    /// 文件相对路径
    /// </summary>
    [FsColumn("文件路径", false, 200)]
    public string FilePath { get; set; }

    /// <summary>
    /// 完整路径
    /// </summary>
    [FsColumn("完整路径", IsIgnore = true)]
    public string FileWebPath
    {
        get
        {
            return FilePath.IsNull() ? "" : App.Configuration[AppSettingsConst.DomainUrl] + FilePath;
        }
    }

    /// <summary>
    /// 前端上传组件回显需要
    /// </summary>
    [FsColumn("前端上传组件回显需要", IsIgnore = true)]
    public string name
    {
        get
        {
            return this.FileName;
        }
    }

    /// <summary>
    /// 前端上传组件回显需要
    /// </summary>
    [FsColumn("前端上传组件回显需要", IsIgnore = true)]
    public string url
    {
        get
        {
            return this.FileWebPath;
        }
    }
}

/// <summary>
/// 附件类型
/// </summary>
public enum CommonAttachType
{
    Demo = 1,
}