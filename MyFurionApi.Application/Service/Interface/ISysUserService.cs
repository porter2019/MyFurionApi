namespace MyFurionApi.Application;

/// <summary>
/// 用户服务
/// </summary>
public interface ISysUserService
{
    /// <summary>
    /// 检查用户是否有指定权限
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <param name="controllerName">接口控制器命名空间完整名称</param>
    /// <param name="operations">要校验的操作,英文逗号分割</param>
    /// <returns></returns>
    Task<bool> CheckHasPermissionAsync(int userId, string controllerName, string operations);

    /// <summary>
    /// 获取有某个权限的用户Id列表
    /// </summary>
    /// <param name="hanlderName">ERPOrderSalesJob1</param>
    /// <param name="actionName">如果为空，则默认置为：show</param>
    /// <returns></returns>
    Task<List<int>> GetPermitUserIdList(string hanlderName, string actionName);
}
