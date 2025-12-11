namespace MyFurionApi.Application.Dto;

public class SysUserChangePwdInput : BaseFormPostModel
{
    /// <summary>
    /// 旧密码
    /// </summary>
    [Required(ErrorMessage = "缺少原密码")]
    public string OldPassword { get; set; }

    /// <summary>
    /// 新密码
    /// </summary>
    [Required(ErrorMessage = "缺少新密码")]
    public string NewPassword { get; set; }
}
