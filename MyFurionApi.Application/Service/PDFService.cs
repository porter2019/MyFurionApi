using Furion.HttpRemote;

namespace MyFurionApi.Application;

/// <summary>
/// PDF服务
/// </summary>
public class PDFService : IPDFService, ITransient
{
    private readonly ILogger<PDFService> _logger;
    private readonly IConfiguration _config;
    private readonly IHttpRemoteService _remoteService;

    public PDFService(ILogger<PDFService> logger, IConfiguration config, IHttpRemoteService remoteService)
    {
        _logger = logger;
        _config = config;
        _remoteService = remoteService;
    }

    /// <summary>
    /// Excel转PDF
    /// </summary>
    /// <param name="excelPath">excel文件的物理路径</param>
    /// <param name="pdfPath">输出pdf的物理路径</param>
    /// <returns></returns>
    public async Task<bool> ExcelToPdf(string excelPath, string pdfPath)
    {
        //Docker 安装服务
        //docker run --name gotenberg -p 5924:3000 --restart=always --network 1panel-network -d gotenberg/gotenberg:8

        try
        {
            if (!File.Exists(excelPath)) return false;
            if (File.Exists(pdfPath)) File.Delete(pdfPath);
            var url = $"{_config["PDFService:Host"]}/forms/libreoffice/convert";
            var response = await _remoteService.PostAsync(url,
                builder => builder
                        .SetMultipartContent(multipart => multipart
                        //.AddFormItem("","")
                        .AddFileAsStream(excelPath, "files")
                        ));

            if (response.IsSuccessStatusCode)
            {
                var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync(pdfPath, pdfBytes);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excel转PDF出现错误");
            return false;
        }
    }

}
