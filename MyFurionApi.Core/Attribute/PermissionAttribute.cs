namespace MyFurionApi.Core;

/// <summary>
/// 权限操作标识
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class PermissionAttribute : Attribute
{
    /// <summary>
    /// 权限标识
    /// </summary>
    public PermissionAttribute() { }

    /// <summary>
    /// 指定操作名和别名
    /// </summary>
    /// <param name="operationName"></param>
    /// <param name="aliasName"></param>
    public PermissionAttribute(string operationName, string aliasName)
    {
        this.OperationName = operationName;
        this.AliasName = aliasName;
    }

    /// <summary>
    /// 指定操作名和别名
    /// </summary>
    /// <param name="operationName"></param>
    /// <param name="aliasName"></param>
    /// <param name="autoCheck"></param>
    public PermissionAttribute(string operationName, string aliasName, bool autoCheck)
    {
        this.OperationName = operationName;
        this.AliasName = aliasName;
        this.AutoCheck = autoCheck;
    }


    /// <summary>
    /// 操作名称
    /// </summary>
    public string OperationName { get; set; }

    /// <summary>
    /// 别名
    /// </summary>
    public string AliasName { get; set; }

    /// <summary>
    /// 自动检查权限
    /// </summary>
    public bool AutoCheck { get; set; } = true;
}
