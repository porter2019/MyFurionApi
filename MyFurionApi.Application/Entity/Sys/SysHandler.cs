namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统模块下的功能
/// </summary>
[FsTable()]
public class SysHandler : BaseEntity
{
    /// <summary>
    /// 所属模块Id
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int ModuleId { get; set; }

    /// <summary>
    /// 功能名称
    /// </summary>
    [FsColumn(50)]
    public string HandlerName { get; set; }

    /// <summary>
    /// 功能别名
    /// </summary>
    [FsColumn(50)]
    public string AliasName { get; set; }

    /// <summary>
    /// 关联控制器
    /// </summary>
    [FsColumn(500)]
    public string RefController { get; set; }

    /// <summary>
    /// 排序数字
    /// </summary>
    [FsColumn(IsNullable = false)]
    public int OrderNo { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool Status { get; set; } = true;
}