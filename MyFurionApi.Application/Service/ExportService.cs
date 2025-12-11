using Furion.HttpRemote;
using Magicodes.ExporterAndImporter.Core;
using Magicodes.ExporterAndImporter.Excel;

namespace MyFurionApi.Application;

/// <summary>
/// 导出服务
/// </summary>
public class ExportService : IExportService, ITransient
{
    private readonly ILogger<ExportService> _logger;
    private readonly IConfiguration _config;
    private readonly IHttpRemoteService _remoteService;

    public ExportService(ILogger<ExportService> logger, IConfiguration config, IHttpRemoteService remoteService)
    {
        _logger = logger;
        _config = config;
        _remoteService = remoteService;
    }

    /// <summary>
    /// 导出到excel或pdf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="exportData">数据</param>
    /// <returns>返回文件的访问地址</returns>
    public async Task<string> Export<T>(string templateName, string fileName, string format, ExcelExportStandardOutput<T> exportData) where T : class
    {
        #region 文件准备

        var webRootPath = App.WebHostEnvironment.WebRootPath;
        var templatePath = Path.Combine(webRootPath, "template", templateName);
        if (!File.Exists(templatePath)) throw Oops.Oh("模板文件不存在");
        var domain = _config.Get<string>(AppSettingsConst.DomainUrl);

        var saveFolder = Path.Combine("uploads", "export", "excel");
        var saveFolderPdf = Path.Combine("uploads", "export", "pdf");
        var saveFolderIO = Path.Combine(webRootPath, saveFolder);
        var saveFolderPdfIO = Path.Combine(webRootPath, saveFolderPdf);

        if (!Directory.Exists(saveFolderIO)) Directory.CreateDirectory(saveFolderIO);
        if (!Directory.Exists(saveFolderPdfIO)) Directory.CreateDirectory(saveFolderPdfIO);

        var newFileRelative = Path.Combine("uploads", "export", "excel", $"{fileName}.xlsx");
        var newFileIOPath = Path.Combine(webRootPath, newFileRelative);
        if (File.Exists(newFileIOPath)) File.Delete(newFileIOPath);

        #endregion

        IExportFileByTemplate exporter = new ExcelExporter();
        await exporter.ExportByTemplate(newFileIOPath, exportData, templatePath);

        if (format.Equals("excel"))
        {
            return domain + "/" + newFileRelative.Replace(Path.DirectorySeparatorChar, '/');
        }
        else
        {
            var pdfRelative = Path.Combine("uploads", "export", "pdf", $"{fileName}.pdf");
            var pdfIOPath = Path.Combine(webRootPath, pdfRelative);

            if (await ExcelToPdf(newFileIOPath, pdfIOPath))
                return domain + "/" + pdfRelative.Replace(Path.DirectorySeparatorChar, '/');
            else
                return domain + "/" + newFileRelative.Replace(Path.DirectorySeparatorChar, '/');
        }
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
