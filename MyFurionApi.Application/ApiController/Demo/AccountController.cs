using Furion.DataEncryption.Extensions;

namespace MyFurionApi.Application.ApiController;

/// <summary>
/// 账户
/// </summary>
public class AccountController : BaseApiController
{
    private readonly ILogger<AccountController> _logger;
    private readonly SqlSugarRepository<UserInfo> _userInfoRep;

    public AccountController(ILogger<AccountController> logger, SqlSugarRepository<UserInfo> userInfoRep)
    {
        _logger = logger;
        _userInfoRep = userInfoRep;
    }

    /// <summary>
    /// 添加测试用户
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public int InitData()
    {
        var userEntity = new UserInfo()
        {
            LoginName = "admin",
            Password = "000".ToMD5Encrypt(),
            UserName = "管理员",
            IsAdmin = true
        };
        _userInfoRep.InsertReturnIdentity(userEntity);
        userEntity = new UserInfo()
        {
            LoginName = "test",
            Password = "000".ToMD5Encrypt(),
            UserName = "测试用户A"
        };
        _userInfoRep.InsertReturnEntity(userEntity);
        return userEntity.Id;
    }

    /// <summary>
    /// 登录/获取JWT Token
    /// </summary>
    /// <param name="loginName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public string Login(string loginName, string password)
    {
        var pwd = password.ToMD5Encrypt();
        var userEntity = _userInfoRep.FirstOrDefault(x => x.LoginName == loginName && x.Password == pwd);
        if (userEntity == null) throw Oops.Oh("用户不存在");
        var token = JWTEncryption.Encrypt(new Dictionary<string, object>()
        {
            { ClaimConst.UserId, userEntity.Id },
            { ClaimConst.UserName, userEntity.UserName },
            { ClaimConst.IsSuperAdmin, userEntity.IsAdmin },
            { ClaimConst.Account, userEntity.LoginName },
        });

        //将token输出到响应头中
        //Knife4jUI 的登录接口中，请求参数后面有一栏“AfterScript”，输入以下代码，自动将后续接口带上token
        //ke.global.setAllHeader(
        //  "Authorization",
        //  "Bearer " + ke.response.headers["access-token"]
        //);
        App.HttpContext.SetTokensOfResponseHeaders(token, "这里是刷新token");
        return token;
    }

}
