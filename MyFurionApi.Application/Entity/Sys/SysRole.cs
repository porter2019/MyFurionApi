namespace MyFurionApi.Application.Entity;

/// <summary>
/// 用户组
/// </summary>
[FsTable()]
public class SysRole : BaseEntityStandard
{
    /// <summary>
    /// 用户组名称
    /// </summary>
    [FsColumn(50)]
    public string Name { get; set; }

    /// <summary>
    /// 排序数字
    /// </summary>
    [FsColumn()]
    public int Sort { get; set; }

    /// <summary>
    /// 是否超级管理组
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool IsSuper { get; set; }
}