using System.IO;

namespace MyFurionApi.Core;

/// <summary>
/// 图形验证码
/// </summary>
public interface IImgCaptchaService
{
    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <param name="code">验证码</param>
    /// <param name="width">宽为0将根据验证码长度自动匹配合适宽度</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    MemoryStream GenerateAsync(string code, int width = 0, int height = 30);

    /// <summary>
    /// 根据操作标识生成图形验证码
    /// </summary>
    /// <remarks>同时保存到缓存中</remarks>
    /// <param name="guid"></param>
    /// <returns></returns>
    Task<MemoryStream> GenerateAsync(string guid);

    /// <summary>
    /// 校验图形验证码
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<bool> ValidateCodeAsync(string guid, string code);
}
