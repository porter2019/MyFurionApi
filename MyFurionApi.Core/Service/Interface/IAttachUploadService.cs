using Microsoft.AspNetCore.Http;

namespace MyFurionApi.Core;

public interface IAttachUploadService
{
    /// <summary>
    /// 保存附件
    /// </summary>
    /// <param name="files">上传文件列表</param>
    /// <param name="folder">保存的文件夹</param>
    /// <returns></returns>
    List<UploadFileInfo> SaveAttach(IFormFileCollection files, string folder);
}
