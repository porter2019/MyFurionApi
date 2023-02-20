namespace MyFurionApi.Application;

/// <summary>
/// 用户服务
/// </summary>
public interface ISysUserService
{
    Task<bool> CheckHasPermissionAsync(int userId, string controllerName, string operations);
}
