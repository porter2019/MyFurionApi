using Furion.DependencyInjection;
using Furion.DistributedIDGenerator;
using Furion.FriendlyException;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MyFurionApi.Core;

/// <summary>
/// 图形验证码服务
/// </summary>
public class ImgCaptchaService : IImgCaptchaService, ITransient
{
    private readonly ILogger<ImgCaptchaService> _logger;
    private readonly ICacheMyService _cacheService;

    private readonly string _cacheKey = "img_captcha_";

    public ImgCaptchaService(ILogger<ImgCaptchaService> logger, IOptions<CacheOptions> cacheOptions, Func<string, ISingleton, object> resolveNamed)
    {
        _logger = logger;
        // 因为都注入了MemoryCache和RedisCache，所以这里根据配置文件来取具体使用哪个服务
        _cacheService = resolveNamed(cacheOptions.Value.CacheType.ToString(), default) as ICacheMyService;
    }

    /// <summary>
    /// 根据操作标识生成图形验证码
    /// </summary>
    /// <remarks>同时保存到缓存中</remarks>
    /// <param name="guid"></param>
    /// <returns></returns>
    public async Task<MemoryStream> GenerateAsync(string guid)
    {
        var cacheKey = _cacheKey + guid;
        var code = RandomHelper.GenerateIntNumber(4);
        var ms = GenerateAsync(code, 0, 30);
        await _cacheService.SetAsync(cacheKey, code, TimeSpan.FromMinutes(60)); //1小时后缓存过期
        return ms;
    }

    /// <summary>
    /// 校验图形验证码
    /// </summary>
    /// <param name="guid"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<bool> ValidateCodeAsync(string guid, string code)
    {
        var cacheKey = _cacheKey + guid;
        var cacheValue = await _cacheService.GetAsync(cacheKey);
        if (cacheValue.IsNull()) throw Oops.Bah("图形验证码无效");

        if (!cacheValue.Equals(code)) throw Oops.Bah("图形验证码输入有误");
        await _cacheService.DelAsync(cacheKey);
        return true;

    }

    /// <summary>
    /// 生成验证码图片
    /// </summary>
    /// <param name="code">验证码</param>
    /// <param name="width">宽为0将根据验证码长度自动匹配合适宽度</param>
    /// <param name="height">高</param>
    /// <remarks>注意引用的System.Drawing.Common包版本是5.0的，6.0的会提示仅支持windows</remarks>
    /// <returns></returns>
    public MemoryStream GenerateAsync(string code, int width = 0, int height = 30)
    {
        //验证码颜色集合
        Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

        //验证码字体集合
        string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

        //定义图像的大小，生成图像的实例

        var image = new Bitmap(width == 0 ? code.Length * 25 : width, height);
        var g = Graphics.FromImage(image);

        //背景设为白色
        g.Clear(Color.White);

        var random = new Random();

        //在随机位置画背景点
        for (var i = 0; i < 100; i++)
        {
            var x = random.Next(image.Width);
            var y = random.Next(image.Height);
            g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
        }

        //验证码绘制在g中
        for (var i = 0; i < code.Length; i++)
        {
            //随机颜色索引值
            var cindex = random.Next(c.Length);

            //随机字体索引值
            var findex = random.Next(fonts.Length);

            //字体
            var f = new Font(fonts[findex], 16, FontStyle.Bold);

            //颜色
            Brush b = new SolidBrush(c[cindex]);

            var ii = 4;
            if ((i + 1) % 2 == 0)//控制验证码不在同一高度
                ii = 2;

            //绘制一个验证字符
            g.DrawString(code.Substring(i, 1), f, b, 17 + (i * 17), ii);
        }

        var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);

        g.Dispose();
        image.Dispose();
        //return File(ms.ToArray(), @"image/png");
        return ms;
    }
}
