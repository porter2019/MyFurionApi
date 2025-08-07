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
    [FsColumn("用户组名称", 50)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [FsColumn("描述", 200)]
    public string Description { get; set; }

    /// <summary>
    /// 是否超级管理组
    /// </summary>
    [FsColumn("是否超级管理组", false)]
    public bool IsSuper { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [FsColumn("状态", false)]
    public bool Status { get; set; } = true;
}