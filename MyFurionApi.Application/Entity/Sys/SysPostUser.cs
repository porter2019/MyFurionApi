namespace MyFurionApi.Application.Entity;

/// <summary>
/// 岗位用户表
/// </summary>
[FsTable]
public class SysPostUser : BaseEntity
{
    /// <summary>
    /// 岗位id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int PostId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int UserId { get; set; }
}
