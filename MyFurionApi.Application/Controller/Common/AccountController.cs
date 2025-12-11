using Furion.DataEncryption.Extensions;

namespace MyFurionApi.Application.Controller;

/// <summary>
/// 账户
/// </summary>
public class AccountController : BaseApiController
{
    private readonly ILogger<AccountController> _logger;
    private readonly SqlSugarRepository<SysUser> _userRepository;
    private readonly IConfiguration _config;
    private readonly IEventPublisher _publisher;

    public AccountController(ILogger<AccountController> logger, SqlSugarRepository<SysUser> userRepository, IConfiguration config, IEventPublisher publisher)
    {
        _logger = logger;
        _userRepository = userRepository;
        _config = config;
        _publisher = publisher;
    }

    /// <summary>
    /// 添加测试用户
    /// </summary>
    /// <returns></returns>
    [HttpGet, AllowAnonymous]
    public string InitData()
    {
        var userEntity = new SysUser()
        {
            CellPhone = "13000000000",
            Password = "000".ToMD5Encrypt(),
            UserName = "管理员",
            IsSuper = true,
            IsOM = true,
            IsMP = true
        };
        if (!_userRepository.Any(x => x.CellPhone == userEntity.CellPhone))
            _userRepository.InsertReturnIdentity(userEntity);
        userEntity = new SysUser()
        {
            CellPhone = "14000000000",
            Password = "000".ToMD5Encrypt(),
            UserName = "测试用户A"
        };
        if (!_userRepository.Any(x => x.CellPhone == userEntity.CellPhone))
            _userRepository.InsertReturnEntity(userEntity);
        return "OK";
    }

    /// <summary>
    /// 登录/获取JWT Token
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public async Task<string> Login(LoginInput req)
    {
        //如果是swaggerUI，到全局参数添加：from  om

        var clientType = CommonHelper.GetClientType();
        if (clientType == ClientFromType.未知) throw Oops.Bah("未知来源(Head缺少from)");

        var pwd = req.Password.ToMD5Encrypt();
        var userEntity = await _userRepository.FirstOrDefaultAsync(x => x.CellPhone == req.CellPhone && x.Password == pwd);
        if (userEntity == null) throw Oops.Bah("手机号或密码不正确");
        if (!userEntity.Status) throw Oops.Bah("用户被禁用");
        if (clientType == ClientFromType.后台 && !userEntity.IsOM) throw Oops.Bah("账号不允许登录管理后台");
        if (clientType == ClientFromType.小程序 && !userEntity.IsMP) throw Oops.Bah("账号不允许登录移动端");
        await _userRepository.UpdateAsync(x => new SysUser() { LastLoginTime = DateTime.Now }, x => x.Id == userEntity.Id);
        var token = GenerateToken(userEntity);

        //将token输出到响应头中
        //Knife4jUI 的登录接口中，请求参数后面有一栏“AfterScript”，输入以下代码，自动将后续接口带上token
        //ke.global.setAllHeader(
        //  "Authorization",
        //  "Bearer " + ke.response.headers["access-token"]
        //);
        App.HttpContext.SetTokensOfResponseHeaders(token, "refresh-token");

        #region 添加登录日志

        await _publisher.PublishAsync("Log:Login:Add", new LogLogin()
        {
            ClientType = CommonHelper.GetClientType(),
            UserId = userEntity.Id,
            UserName = userEntity.UserName,
            CellPhone = userEntity.CellPhone,
            //Platform = platform,
            IP = App.HttpContext.GetClientIpAddress(),
            CreatedTime = DateTime.Now,
            UserAgent = App.HttpContext.Request.Headers["User-Agent"].ToString(),
        });

        #endregion

        return token;
    }

    /// <summary>
    /// 判断手机号是否被注册
    /// </summary>
    /// <param name="cellPhone"></param>
    /// <remarks>返回False表示该手机号没有注册</remarks>
    /// <returns></returns>
    [HttpGet, Route("check/exists/cellphone"), AllowAnonymous, SwaggerIgnore]
    public Task<bool> CheckCellPhoneAny(string cellPhone)
    {
        return _userRepository.AnyAsync(x => x.CellPhone.Equals(cellPhone));
    }

    #region 微信小程序

    ///// <summary>
    ///// 根据手机号的动态令牌获取手机号做自动登录
    ///// </summary>
    ///// <param name="phoneCode">小程序调用getPhoneNumber获得的code</param>
    ///// <param name="fakePhone">假的的手机号，用于本地测试</param>
    ///// <returns></returns>
    //[HttpGet, Route("wx/applet/phonecode"), AllowAnonymous]
    //public string LoginWithPhoneCode(string phoneCode, string fakePhone)
    //{
    //    //if (!_config.GetValue<bool>("SenparcSetting:IsEnabled")) throw Oops.Oh("管理员已禁用微信登录");
    //    if (phoneCode.IsNull()) throw Oops.Oh("动态令牌不能为空");

    //    string appId = _config.Get<string>("OpenOAuth:WeChatMiniProgram:AppID");
    //    var phoneInfo = BusinessApi.GetUserPhoneNumber(appId, phoneCode);
    //    _logger.LogDebug("【小程序手机号授权登录】返回值：" + JsonHelper.Serialize(phoneInfo));
    //    if (phoneInfo.phone_info.phoneNumber.IsNull()) throw Oops.Oh("获取手机号失败");

    //    var cellPhone = (_env.IsDevelopment() && fakePhone.IsNotNull()) ? fakePhone : phoneInfo.phone_info.phoneNumber;
    //    var userEntity = _userRepository.FirstOrDefault(x => x.CellPhone == cellPhone);
    //    if (userEntity == null) throw Oops.Oh("用户不存在");
    //    if (!userEntity.Status) throw Oops.Oh("用户已被禁用");
    //    _userRepository.Update(x => new SysUser() { LastLoginTime = DateTime.Now }, x => x.Id == userEntity.Id);
    //    return GenerateToken(userEntity);
    //}

    ///// <summary>
    ///// 获取用户的OpenId
    ///// </summary>
    ///// <param name="code"></param>
    ///// <returns></returns>
    //[HttpGet, Route("wx/applet/getopenid"), AllowAnonymous]
    //public dynamic GetWXOpenId(string code)
    //{
    //    if (!_config.GetValue<bool>("SenparcSetting:IsEnabled")) throw Oops.Oh("管理员已禁用微信登录");
    //    if (code.IsNull()) throw Oops.Oh("缺少参数");

    //    string appId = _config.Get<string>("OpenOAuth:WeChatMiniProgram:AppID");
    //    string appSecret = _config.Get<string>("OpenOAuth:WeChatMiniProgram:AppSecret");
    //    string openId, unionId, sessionId;

    //    #region 换取用户openId

    //    var jsonResult = SnsApi.JsCode2Json(appId, appSecret, code);
    //    if (jsonResult.errcode == Senparc.Weixin.ReturnCode.请求成功)
    //    {
    //        unionId = jsonResult.unionid;
    //        openId = jsonResult.openid;
    //        var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key, unionId);
    //        sessionId = sessionBag.Key;
    //    }
    //    else
    //    {
    //        _logger.LogError($"换取用户标识失败：{JsonHelper.Serialize(jsonResult)}");
    //        throw Oops.Oh("换取用户标识失败");
    //    }

    //    return openId;

    //    #endregion

    //    //SysOAuth oAuthEntity = null;

    //    //#region 添加或修改 SysOAuth

    //    //var oldOAuthEntity = await _oauthRep.FirstOrDefaultAsync(x => x.OpenId == openId);

    //    //if (oldOAuthEntity == null)
    //    //{
    //    //    oldOAuthEntity = new SysOAuth();
    //    //    //完全第一次授权登录系统
    //    //    oldOAuthEntity.OpenId = openId;
    //    //    oldOAuthEntity.UnionId = unionId;
    //    //    oldOAuthEntity.City = req.city;
    //    //    oldOAuthEntity.Country = req.country;
    //    //    oldOAuthEntity.Gender = req.gender;
    //    //    oldOAuthEntity.Language = req.language;
    //    //    oldOAuthEntity.NickName = req.nickName;
    //    //    oldOAuthEntity.Province = req.province;
    //    //    oAuthEntity = await _oauthRep.InsertReturnEntityAsync(oldOAuthEntity);

    //    //}
    //    //else
    //    //{
    //    //    //象征性的更新一下资料
    //    //    oldOAuthEntity.UnionId = unionId;
    //    //    oldOAuthEntity.City = req.city;
    //    //    oldOAuthEntity.Country = req.country;
    //    //    oldOAuthEntity.Gender = req.gender;
    //    //    oldOAuthEntity.Language = req.language;
    //    //    oldOAuthEntity.NickName = req.nickName;
    //    //    oldOAuthEntity.Province = req.province;
    //    //    await _oauthRep.UpdateAsync(oldOAuthEntity, "OpenId");
    //    //    oAuthEntity = oldOAuthEntity;
    //    //}

    //    //#endregion

    //    ////如果该OAuth绑定了用户，则做登录，返回Token，否则继续走获取手机号操作
    //    //if (oAuthEntity.UserId > 0)
    //    //{
    //    //    //如果已经绑定了用户，则做登录操作，返回token
    //    //    var token = await GetAccessTokenByUserId(oAuthEntity.UserId);
    //    //    return new { AccessToken = token, OAuthId = oAuthEntity.Id, SessionId = sessionId };
    //    //}
    //    //else
    //    //{
    //    //    //没有绑定用户，返回SessionId，走获取手机号操作
    //    //    return new { AccessToken = "", OAuthId = oAuthEntity.Id, SessionId = sessionId };
    //    //}
    //}


    #endregion

    #region 公众号

    ///// <summary>
    ///// 根据微信的code自动绑定公众号
    ///// </summary>
    ///// <param name="code"></param>
    ///// <param name="userId"></param>
    ///// <returns></returns>
    //[HttpGet, Route("wx/mp/bind"), AllowAnonymous]
    //public async Task<bool> BindWxMPByCode(string code, int userId)
    //{
    //    if (code.IsNull()) throw Oops.Oh("缺少code参数");
    //    var userEntity = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
    //    if (userEntity == null) throw Oops.Oh("用户不存在");

    //    var appId = _config["OpenOAuth:WeChatOpenServices:AppID"];
    //    var appSecret = _config["OpenOAuth:WeChatOpenServices:AppSecret"];

    //    try
    //    {
    //        var result = OAuthApi.GetAccessToken(appId, appSecret, code);
    //        if (result.errcode == ReturnCode.请求成功)
    //        {
    //            if (!result.openid.Equals(userEntity.WxMPOpenId))
    //            {
    //                _logger.LogInformation($"【用户绑定公众号】 用户Id：{userEntity.Id}，原绑定标识：{userEntity.WxMPOpenId}, 新的公众号标识：{result.openid}");
    //                await _userRepository.UpdateAsync(x => new SysUser() { WxMPOpenId = result.openid }, x => x.Id == userEntity.Id);
    //                return true;
    //            }
    //            else
    //            {
    //                _logger.LogInformation($"【用户绑定公众号】 用户Id：{userEntity.Id}，新获取公众号标识：{result.openid}与原绑定标识一致，不做修改");
    //                return true;
    //            }
    //        }
    //        else
    //        {
    //            _logger.LogError($"公众号H5换取用户标识失败：{JsonHelper.Serialize(result)}");
    //            throw Oops.Oh(result.errmsg);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        if (!ex.Message.Contains("code been used"))
    //        {
    //            _logger.LogError(ex, "换取用户openid失败");
    //            throw Oops.Oh(ex.Message);
    //        }
    //        return false;
    //    }

    //}

    #endregion

    #region 私有方法

    /// <summary>
    /// 私有方法-根据用户Id做登录，获取token
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    async Task<string> GetAccessTokenByUserId(int userId)
    {
        var userInfo = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
        if (userInfo == null) throw Oops.Bah("用户不存在");
        await _userRepository.UpdateAsync(x => new SysUser() { LastLoginTime = DateTime.Now }, x => x.Id == userInfo.Id);
        return GenerateToken(userInfo);
    }

    /// <summary>
    /// 私有方法-生成JWT Token
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    /// <returns></returns>
    string GenerateToken(SysUser userInfo)
    {
        return JWTEncryption.Encrypt(new Dictionary<string, object>()
        {
            { ClaimConst.UserId, userInfo.Id },
            { ClaimConst.UserName, userInfo.UserName },
            { ClaimConst.IsSuperAdmin, userInfo.IsSuper },
            { ClaimConst.Account, userInfo.CellPhone },
        });
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

    #endregion
}
