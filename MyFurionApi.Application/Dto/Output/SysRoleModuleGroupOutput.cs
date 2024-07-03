namespace MyFurionApi.Application.Dto;

/// <summary>
/// 用户组按模块分组显示
/// </summary>
public class SysRoleModuleGroupOutput
{
    public SysRoleModuleGroupOutput(string moduleName)
    {
        this.ModuleName = moduleName;
        this.HandlerList = new List<SysRoleHandlerGroupOutput>();
    }

    /// <summary>
    /// 模块名称
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// 功能列表
    /// </summary>
    public List<SysRoleHandlerGroupOutput> HandlerList { get; set; }
}

/// <summary>
/// 用户组按控制器分组
/// </summary>
public class SysRoleHandlerGroupOutput
{
    public SysRoleHandlerGroupOutput(string handlerName)
    {
        this.HandlerName = handlerName;
        this.PermitList = new List<SysRolePermitOutput>();
    }

    /// <summary>
    /// 功能名称
    /// </summary>
    public string HandlerName { get; set; }

    /// <summary>
    /// 排序数字
    /// </summary>
    public int OrderNo { get; set; }

    /// <summary>
    /// 权限列表
    /// </summary>
    public List<SysRolePermitOutput> PermitList { get; set; }
}

/// <summary>
/// 用户组的权限信息
/// </summary>
public class SysRolePermitOutput
{
    /// <summary>
    /// 模块名称
    /// </summary>
    public string ModuleName { get; set; }

    /// <summary>
    /// 功能名称
    /// </summary>
    public string HandlerName { get; set; }

    /// <summary>
    /// 操作id
    /// </summary>
    public int PermitId { get; set; }

    /// <summary>
    /// 操作名称
    /// </summary>
    public string PermitName { get; set; }

    /// <summary>
    /// 操作别名
    /// </summary>
    public string AliasName { get; set; }

    /// <summary>
    /// 排序数字
    /// </summary>
    public int OrderNo { get; set; }

    /// <summary>
    /// 组是否有了该权限
    /// </summary>
    public bool IsChecked { get; set; }
}
