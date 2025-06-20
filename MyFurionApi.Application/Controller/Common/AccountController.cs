using Furion.DataEncryption.Extensions;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 账户
/// </summary>
public class AccountController : BaseApiController
{
    private readonly ILogger<AccountController> _logger;
    private readonly SqlSugarRepository<SysUser> _userInfoRep;

    public AccountController(ILogger<AccountController> logger, SqlSugarRepository<SysUser> userInfoRep)
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
        var userEntity = new SysUser()
        {
            LoginName = "admin",
            Password = "000".ToMD5Encrypt(),
            UserName = "管理员",
            IsAdmin = true
        };
        if (!_userInfoRep.Any(x => x.LoginName == userEntity.LoginName))
            _userInfoRep.InsertReturnIdentity(userEntity);
        userEntity = new SysUser()
        {
            LoginName = "test",
            Password = "000".ToMD5Encrypt(),
            UserName = "测试用户A"
        };
        if (!_userInfoRep.Any(x => x.LoginName == userEntity.LoginName))
            _userInfoRep.InsertReturnEntity(userEntity);
        return userEntity.Id;
    }

    /// <summary>
    /// 登录/获取JWT Token
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public string Login(LoginInput req)
    {
        var pwd = req.Password.ToMD5Encrypt();
        var userEntity = _userInfoRep.FirstOrDefault(x => x.LoginName == req.LoginName && x.Password == pwd);
        if (userEntity == null) throw Oops.Bah("用户不存在");
        _userInfoRep.EntityContext.Updateable<SysUser>().SetColumns("LastLoginTime", DateTime.Now).Where(x => x.Id == userEntity.Id).ExecuteCommand();
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
        App.HttpContext.SetTokensOfResponseHeaders(token, "refresh-token");
        return token;
    }

    ///// <summary>
    ///// 手动验证某个字段
    ///// </summary>
    ///// <returns></returns>
    //[HttpGet]
    //public bool Manual(string account)
    //{
    //    //官方自带验证规则：ValidationTypes.PhoneNumber
    //    //填充
    //    UnifyContext.Fill(new { Enum = "枚举列表" });
    //    return account.TryValidate(MyValidationTypes.AccountName).IsValid;
    //}

}
