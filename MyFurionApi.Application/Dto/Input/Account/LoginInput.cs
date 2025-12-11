namespace MyFurionApi.Application.Dto;

/// <summary>
/// 账号登录所需参数
/// </summary>
public class LoginInput
{
    ///// <summary>
    ///// 登录名
    ///// </summary>
    //[Required(ErrorMessage = "登录名必填"), DataValidation(MyValidationTypes.AccountName)]
    //public string LoginName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Required(ErrorMessage = "手机号必填"), DataValidation(MyValidationTypes.CellPhone)]
    public string CellPhone { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [Required(ErrorMessage = "密码必填"), MinLength(3, ErrorMessage = "最少3个字符"), MaxLength(30, ErrorMessage = "最多30个字符")]
    public string Password { get; set; }
}
