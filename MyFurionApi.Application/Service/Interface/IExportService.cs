

namespace MyFurionApi.Application;

public interface IExportService
{
    /// <summary>
    /// Excel转PDF
    /// </summary>
    /// <param name="excelPath">excel文件的物理路径</param>
    /// <param name="pdfPath">输出pdf的物理路径</param>
    /// <returns></returns>
    Task<bool> ExcelToPdf(string excelPath, string pdfPath);

    /// <summary>
    /// 导出到excel或pdf
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="exportData">数据</param>
    /// <returns>返回文件的访问地址</returns>
    Task<string> Export<T>(string templateName, string fileName, string format, ExcelExportStandardOutput<T> exportData) where T : class;
}
