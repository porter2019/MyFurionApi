namespace MyFurionApi.Application.Controller;


/// <summary>
/// 上下文信息
/// </summary>
public class CurrentController : BaseApiController
{
    private readonly ILogger<CurrentController> _logger;
    private readonly SqlSugarRepository<SysUser> _sysUserRepository;

    public CurrentController(ILogger<CurrentController> logger, SqlSugarRepository<SysUser> sysUserRepository)
    {
        _logger = logger;
        _sysUserRepository = sysUserRepository;
    }

    /// <summary>
    /// 获取上下文中用户信息
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/user/info")]
    public dynamic GetUserInfo()
    {
        var userId = (App.User.FindFirst(ClaimConst.UserId)?.Value).ObjToInt();
        var userInfo = _sysUserRepository.FirstOrDefault(x => x.Id == userId);
        if (userInfo == null) throw Oops.Oh("用户不存在");
        if (!userInfo.Status) throw Oops.Oh("用户已被禁用");

        return new
        {
            UserId = userInfo.Id,
            //CellPhone = userInfo.CellPhone,
            LoginName = userInfo.LoginName,
            UserName = userInfo.UserName,
            Avatar = userInfo.AvatarPath,
            LastLoginTime = userInfo.LastLoginTime,
            userInfo.IsAdmin,
        };
    }

    /// <summary>
    /// 获取用户权限信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("get/permission/list")]
    public List<string> GetPermissionList()
    {
        //var isAdmin = (App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value).ObjToBool();
        //超管
        return new List<string>() { "ALL" };
    }

}
