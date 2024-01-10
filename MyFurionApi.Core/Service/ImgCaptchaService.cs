using Furion.DependencyInjection;
using Furion.FriendlyException;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkiaSharp;
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

    public ImgCaptchaService(ILogger<ImgCaptchaService> logger, IOptions<CacheOptions> cacheOptions, INamedServiceProvider<ICacheMyService> namedServiceProvider)
    {
        _logger = logger;
        // 因为都注入了MemoryCache和RedisCache，所以这里根据配置文件来取具体使用哪个服务
        _cacheService = namedServiceProvider.GetService<ISingleton>(cacheOptions.Value.CacheType.ToString());
    }

    /// <summary>
    /// 根据操作标识生成图形验证码
    /// </summary>
    /// <remarks>同时保存到缓存中</remarks>
    /// <param name="guid"></param>
    /// <returns></returns>
    public async Task<byte[]> GenerateAsync(string guid)
    {
        var cacheKey = _cacheKey + guid;
        var code = RandomHelper.GenerateIntNumber(4);
        var imgByte = GenerateImg(code);
        await _cacheService.SetAsync(cacheKey, code, TimeSpan.FromMinutes(60)); //1小时后缓存过期
        return imgByte;
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
    /// <param name="width">宽</param>
    /// <param name="height">高</param>
    /// <returns></returns>
    public byte[] GenerateImg(string code, int width = 128, int height = 45)
    {
        Random random = new();

        //创建bitmap位图
        using SKBitmap image = new(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
        //创建画笔
        using SKCanvas canvas = new(image);
        //填充背景颜色为白色
        canvas.DrawColor(SKColors.White);

        //画图片的背景噪音线
        for (int i = 0; i < (width * height * 0.015); i++)
        {
            using SKPaint drawStyle = new();
            drawStyle.Color = new(Convert.ToUInt32(random.Next(Int32.MaxValue)));

            canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
        }
        //将文字写到画布上
        //使用本项目的字体文件
        var fontFilePath = Path.Combine(App.WebHostEnvironment.WebRootPath, "fonts", "STXINGKA.ttf");
        var font = new SKFont(SKFontManager.Default.CreateTypeface(File.Open(fontFilePath, FileMode.Open)));
        font.Size = 38;
        using (SKPaint drawStyle = new())
        {
            drawStyle.Color = new SKColor(59, 59, 59);// SKColors.Red;
            drawStyle.TextSize = height - 10;
            drawStyle.StrokeWidth = 1;

            float emHeight = height - (float)height * (float)0.14;
            float emWidth = ((float)width / code.Length) - ((float)width * (float)0.13);
            canvas.DrawText(code, emWidth, emHeight, font, drawStyle);
        }

        //画图片的前景噪音点
        for (int i = 0; i < (width * height * 0.15); i++)
        {
            image.SetPixel(random.Next(0, width), random.Next(0, height), new SKColor(Convert.ToUInt32(random.Next(Int32.MaxValue))));
        }

        using var img = SKImage.FromBitmap(image);
        using SKData p = img.Encode(SKEncodedImageFormat.Png, 100);

        return p.ToArray();
    }
}
