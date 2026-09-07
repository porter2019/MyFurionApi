using Furion.HttpRemote;
using ClosedXML.Excel;

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
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="exportData">数据</param>
    /// <example>https://github.com/mini-software/MiniExcel/blob/master/README.zh-CN.md</example>
    /// <returns>返回文件的访问地址</returns>
    public async Task<string> Export(string templateName, string fileName, string format, object exportData)
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

        await MiniExcelLibs.MiniExcel.SaveAsByTemplateAsync(newFileIOPath, templatePath, exportData);

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
    /// 使用ClosedXML导出excel或PDF
    /// </summary>
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="fillData">每行自定义操作</param>
    /// <example>https://github.com/ClosedXML/ClosedXML</example>
    /// <returns></returns>
    public async Task<string> Export(string templateName, string fileName, string format, Action<XLWorkbook> fillData)
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

        //具体调用的类来操作
        using (var workbook = new XLWorkbook(templatePath))
        {
            fillData(workbook);
            workbook.SaveAs(newFileIOPath);
        }


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
    /// 设置单元格常用样式
    /// </summary>
    /// <param name="cell">单元格</param>
    /// <param name="horizontal">水平对齐</param>
    /// <param name="vertical">垂直对齐</param>
    /// <param name="fontSize">文字大小</param>
    /// <param name="fontColor">文字颜色</param>
    /// <param name="bold">是否加粗</param>
    /// <param name="wapText">是否自动换行</param>
    /// <param name="border">是否加边框</param>
    /// <param name="bgColor">背景色</param>
    public void SetCellStyle(IXLCell cell, XLAlignmentHorizontalValues horizontal = XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues vertical = XLAlignmentVerticalValues.Center, int fontSize = 11, string fontColor = "#000000", bool bold = false, bool wapText = false, bool border = true, string bgColor = null)
    {
        cell.Style.Alignment.Horizontal = horizontal;
        cell.Style.Alignment.Vertical = vertical;
        cell.Style.Alignment.WrapText = wapText;
        cell.Style.Font.FontSize = fontSize;
        cell.Style.Font.Bold = bold;
        cell.Style.Font.FontColor = XLColor.FromHtml(fontColor);
        if (border)
        {
            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetOutsideBorderColor(XLColor.Black);
        }
        if (bgColor.IsNotNull())
        {
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor);
        }
    }

    /// <summary>
    /// 设置单元格常用样式
    /// </summary>
    /// <param name="cell">单元格</param>
    /// <param name="horizontal">水平对齐</param>
    /// <param name="vertical">垂直对齐</param>
    /// <param name="fontSize">文字大小</param>
    /// <param name="fontColor">文字颜色</param>
    /// <param name="bold">是否加粗</param>
    /// <param name="wapText">是否自动换行</param>
    /// <param name="border">是否加边框</param>
    /// <param name="bgColor">背景色</param>
    public void SetCellStyle(IXLRange cell, XLAlignmentHorizontalValues horizontal = XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues vertical = XLAlignmentVerticalValues.Center, int fontSize = 11, string fontColor = "#000000", bool bold = false, bool wapText = false, bool border = true, string bgColor = null)
    {
        cell.Style.Alignment.Horizontal = horizontal;
        cell.Style.Alignment.Vertical = vertical;
        cell.Style.Alignment.WrapText = wapText;
        cell.Style.Font.FontSize = fontSize;
        cell.Style.Font.Bold = bold;
        cell.Style.Font.FontColor = XLColor.FromHtml(fontColor);
        if (border)
        {
            cell.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
            cell.Style.Border.SetOutsideBorderColor(XLColor.Black);
        }
        if (bgColor.IsNotNull())
        {
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(bgColor);
        }
    }

    /// <summary>
    /// 根据文本内容长度获取行高
    /// </summary>
    /// <param name="content">文本内容</param>
    /// <param name="baseHeight">基础行高（字符较短时用这个）</param>
    /// <param name="threshold">超过多少字符开始增高</param>
    /// <param name="extraPerChar">每多1个字符增加多少高度</param>
    /// <returns></returns>
    public double GetRowHeightByLength(string content, double baseHeight = 38, int threshold = 20, double extraPerChar = 1.0)
    {
        if (string.IsNullOrEmpty(content)) return baseHeight;
        int len = content.Length;
        if (len <= threshold) return baseHeight;
        return baseHeight + (len - threshold) * extraPerChar;
    }

    /// <summary>
    /// 获取图片的绝对地址
    /// <param name="relativePath">相对路径，/uploads/a/b.jpg</param>
    /// </summary>
    public string GetAbsolutePath(string relativePath)
    {
        if (relativePath.IsNull()) return null;
        var filePath = relativePath.TrimStart('/', '\\');//不能以/开头
        var webRootPath = App.WebHostEnvironment.WebRootPath;
        var fileIOPath = Path.Combine(webRootPath, filePath);
        return System.IO.File.Exists(fileIOPath) ? fileIOPath : null;
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
