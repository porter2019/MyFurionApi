namespace MyFurionApi.Application.Dto;

/// <summary>
/// 修改附件需要传入的
/// </summary>
public class AttachModifyInput
{
    /// <summary>
    /// 类型
    /// </summary>
    public CommonAttachType AttachType { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    public string FileType { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件后缀名
    /// </summary>
    public string FileExt { get; set; }

    /// <summary>
    /// 文件相对路径
    /// </summary>
    public string FilePath { get; set; }
}
