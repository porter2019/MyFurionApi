namespace MyFurionApi.Application.Controller;

/// <summary>
/// 文件
/// </summary>
public class FileController : BaseApiController
{
    private readonly ILogger<FileController> _logger;
    private readonly IAttachUploadService _uploadService;

    public FileController(ILogger<FileController> logger, IAttachUploadService uploadService)
    {
        _logger = logger;
        _uploadService = uploadService;
    }

    /// <summary>
    /// 通用文件上传
    /// </summary>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public List<UploadFileInfo> Upload()
    {
        var files = App.HttpContext.Request.Form.Files;
        var tag = App.HttpContext.Request.Form["tag"].ToString();
        var result = _uploadService.SaveAttach(files, tag);
        return result;
    }

}
