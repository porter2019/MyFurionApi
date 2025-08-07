using Furion.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace MyFurionApi.Core;

/// <summary>
/// 附件上传服务
/// </summary>
public class AttachUploadService : IAttachUploadService, ISingleton
{
    private readonly IConfiguration _config;

    public AttachUploadService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// 保存附件
    /// </summary>
    /// <param name="files">上传文件列表</param>
    /// <param name="folder">保存的文件夹</param>
    /// <returns></returns>
    public List<UploadFileInfo> SaveAttach(IFormFileCollection files, string folder)
    {
        if (files == null) return default;
        if (string.IsNullOrWhiteSpace(folder)) folder = "attach";
        var baseIODirectory = App.WebHostEnvironment.WebRootPath;
        var domain = _config["AppSettings:DomainUrl"];
        var baseRootFolder = "uploads";
        var pathArr = folder.Split('/').ToList();
        // 将baseRootFolder参数添加到pathArr第一个值
        pathArr.Insert(0, baseRootFolder);
        var saveFolder = Path.Combine([.. pathArr]);  // 使用 Path.Combine 拼接路径

        List<UploadFileInfo> fileList = new();
        foreach (var file in files)
        {
            var fileExt = Path.GetExtension(file.FileName).ToLower(); // 文件后缀名 .jpg
            var fileName = Guid.NewGuid().ToString("N");
            var filePath = $"/{saveFolder}/{fileName}{fileExt}".Replace("\\", "/"); // 保存的相对路径

            if (string.IsNullOrWhiteSpace(fileExt) && file.FileName == "blob") fileExt = ".jpg";

            // 使用 Path.Combine 构建临时文件的绝对路径
            var tempFileIOFolder = Path.Combine(baseIODirectory, baseRootFolder, folder);

            // 创建目录，兼容 Windows 和 Linux
            if (!Directory.Exists(tempFileIOFolder)) Directory.CreateDirectory(tempFileIOFolder);

            var tempFileIOPath = Path.Combine(tempFileIOFolder, $"{fileName}{fileExt}");

            //保存
            using (FileStream fs = new(tempFileIOPath, FileMode.Create))
            {
                file.CopyTo(fs);
                fs.Flush();
                fs.Close();
            }

            //var fileName = Guid.NewGuid().ToString("N");

            //var finalFileIOPath = Path.Combine(baseIODirectory, baseRootFolder, folder, fileName + fileExt);
            //if (File.Exists(finalFileIOPath))
            //{
            //    File.Delete(tempFileIOPath);
            //}
            //else
            //{
            //    File.Move(tempFileIOPath, finalFileIOPath, true);
            //}
            //var finalFilePath = "/" + saveFolder + "/" + md5CR32 + fileExt;

            fileList.Add(new UploadFileInfo()
            {
                FileExt = fileExt,
                FilePath = filePath,
                FileSourceName = file.FileName,
                FileSize = file.Length,
                FileType = file.ContentType,
                FileName = file.FileName,
                FileWebPath = domain + filePath
            });
        }

        return fileList;
    }
}
