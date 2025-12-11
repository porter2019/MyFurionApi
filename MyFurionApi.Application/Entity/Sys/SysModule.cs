namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统模块
/// </summary>
[FsTable()]
public class SysModule : BaseEntity
{
    public SysModule()
    { }

    public SysModule(string moduleName)
    {
        this.ModuleName = moduleName;
    }

    /// <summary>
    /// 模块名称
    /// </summary>
    [FsColumn(50)]
    public string ModuleName { get; set; }

    /// <summary>
    /// 排序数字，降序排列
    /// </summary>
    [FsColumn()]
    public int OrderNo { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [FsColumn()]
    public bool Status { get; set; } = true;
}