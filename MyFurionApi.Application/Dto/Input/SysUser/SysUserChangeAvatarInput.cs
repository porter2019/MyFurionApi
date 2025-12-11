namespace MyFurionApi.Application.Dto;

/// <summary>
/// 修改头像
/// </summary>
public class SysUserChangeAvatarInput : BaseFormPostModel
{
    /// <summary>
    /// 图片路径
    /// </summary>
    [Required(ErrorMessage = "缺少文件")]
    public string Avatar { get; set; }
}
