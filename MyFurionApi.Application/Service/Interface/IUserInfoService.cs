namespace MyFurionApi.Application;

/// <summary>
/// 用户服务
/// </summary>
public interface IUserInfoService
{
    Task<bool> CheckHasPermissionAsync(int userId, string controllerName, string operations);
}
