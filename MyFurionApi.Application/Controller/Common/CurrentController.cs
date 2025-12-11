using Furion.DataEncryption.Extensions;

namespace MyFurionApi.Application.Controller;


/// <summary>
/// 上下文信息
/// </summary>
public class CurrentController : BaseApiController
{
    private readonly ILogger<CurrentController> _logger;
    private readonly SqlSugarRepository<SysUser> _sysUserRepository;
    private readonly ISysRoleService _sysRoleService;

    public CurrentController(ILogger<CurrentController> logger, SqlSugarRepository<SysUser> sysUserRepository, ISysRoleService sysRoleService)
    {
        _logger = logger;
        _sysUserRepository = sysUserRepository;
        _sysRoleService = sysRoleService;
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
            CellPhone = userInfo.CellPhone,
            LoginName = userInfo.LoginName,
            UserName = userInfo.UserName,
            Avatar = userInfo.AvatarPath,
            LastLoginTime = userInfo.LastLoginTime,
            userInfo.IsSuper,
        };
    }

    /// <summary>
    /// 获取用户权限信息
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("get/permission/list")]
    public Task<List<string>> GetPermissionList()
    {
        var isAdmin = (App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value).ObjToBool();
        if (isAdmin)
            return Task.FromResult(new List<string>() { "ALL" });
        else
        {
            return _sysRoleService.GetPermissionsByUserId(CurrentUserId);
        }
    }

    /// <summary>
    /// 修改用户名
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    [HttpGet, Route("modify/username")]
    public async Task<string> ChangeUserName(string userName)
    {
        if (userName.IsNull()) throw Oops.Bah("名称不能为空");
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            UserName = userName,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == CurrentUserId, new LogAction()
        {
            Local = "系统",
            ExtraHandler = "用户",
            ExtraInfo = $"更新后的用户名：{userName}",
            ClientType = CommonHelper.GetClientType(),
        });

        return "修改成功";
    }

    /// <summary>
    /// 修改头像
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, Route("modify/avatar")]
    public async Task<string> ChangeAvatar(SysUserChangeAvatarInput req)
    {
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            Avatar = req.Avatar,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == CurrentUserId, new LogAction()
        {
            Local = "系统",
            ExtraHandler = "用户",
            ExtraInfo = $"更新后的用户头像地址：{req.Avatar}",
            ClientType = CommonHelper.GetClientType(),
        });

        return "修改成功";
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, Route("modify/password")]
    public async Task<string> ChangePassword(SysUserChangePwdInput req)
    {
        var model = await _sysUserRepository.FirstOrDefaultAsync(x => x.Id == CurrentUserId);
        if (model == null) throw Oops.Oh("账号异常");
        if (!model.Status) throw Oops.Bah("账号已被停用");
        var oldPwd = req.OldPassword.ToMD5Encrypt();
        var newPwd = req.NewPassword.ToMD5Encrypt();
        if (!oldPwd.Equals(model.Password)) throw Oops.Bah("原密码错误");
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            Password = newPwd,
            UpdatedUserId = req.CurrentUserId,
            UpdatedUserName = req.CurrentUserName,
            UpdatedTime = DateTime.Now
        }, x => x.Id == CurrentUserId, new LogAction()
        {
            Local = "系统",
            ExtraHandler = "用户",
            ExtraInfo = $"用户修改了密码，用户名：{model.UserName}，用户手机号：{model.CellPhone}",
            ClientType = CommonHelper.GetClientType(),
        });


        return "修改成功";
    }

}
