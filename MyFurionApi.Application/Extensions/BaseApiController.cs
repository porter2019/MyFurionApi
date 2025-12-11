namespace MyFurionApi.Application;

[Route("api/[controller]")]
[ApiDescriptionSettings("Default")]
public class BaseApiController : IDynamicApiController
{
    /// <summary>
    /// 当前登录的用户Id
    /// </summary>
    protected readonly int CurrentUserId;

    /// <summary>
    /// 当前登录的用户名
    /// </summary>
    protected readonly string CurrentUserName;

    /// <summary>
    /// 当前登录的用户的手机号
    /// </summary>
    protected readonly string CurrentCellPhone;

    /// <summary>
    /// 当前登录的用户是否是超级管理员
    /// </summary>
    protected readonly bool CurrentUserIsSuperAdmin;

    public BaseApiController()
    {
        CurrentUserId = App.User.FindFirst(ClaimConst.UserId)?.Value.ObjToInt() ?? 0;
        CurrentUserName = App.User.FindFirst(ClaimConst.UserName)?.Value ?? "";
        CurrentCellPhone = App.User.FindFirst(ClaimConst.Account)?.Value ?? "";
        CurrentUserIsSuperAdmin = App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value.ObjToBool() ?? false;
    }
}
