

using ClosedXML.Excel;

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
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="exportData">数据</param>
    /// <example>https://github.com/mini-software/MiniExcel/blob/master/README.zh-CN.md</example>
    /// <returns>返回文件的访问地址</returns>
    Task<string> Export(string templateName, string fileName, string format, object exportData);

    /// <summary>
    /// 使用ClosedXML导出excel或PDF
    /// </summary>
    /// <param name="templateName">导出模板.xlsx</param>
    /// <param name="fileName">$"Demo-{DateTime.Now.ToFileTime()}"</param>
    /// <param name="format">excel或pdf</param>
    /// <param name="fillData">每行自定义操作</param>
    /// <example>https://github.com/ClosedXML/ClosedXML</example>
    /// <returns></returns>
    Task<string> Export(string templateName, string fileName, string format, Action<XLWorkbook> fillData);

    /// <summary>
    /// 获取图片的绝对地址
    /// <param name="relativePath">相对路径，/uploads/a/b.jpg</param>
    /// </summary>
    string GetAbsolutePath(string relativePath);

    /// <summary>
    /// 根据文本内容长度获取行高
    /// </summary>
    /// <param name="content">文本内容</param>
    /// <param name="baseHeight">基础行高（字符较短时用这个）</param>
    /// <param name="threshold">超过多少字符开始增高</param>
    /// <param name="extraPerChar">每多1个字符增加多少高度</param>
    /// <returns></returns>
    double GetRowHeightByLength(string content, double baseHeight = 38, int threshold = 20, double extraPerChar = 1);

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
    void SetCellStyle(IXLCell cell, XLAlignmentHorizontalValues horizontal = XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues vertical = XLAlignmentVerticalValues.Center, int fontSize = 11, string fontColor = "#000000", bool bold = false, bool wapText = false, bool border = true, string bgColor = null);

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
    void SetCellStyle(IXLRange cell, XLAlignmentHorizontalValues horizontal = XLAlignmentHorizontalValues.Center, XLAlignmentVerticalValues vertical = XLAlignmentVerticalValues.Center, int fontSize = 11, string fontColor = "#000000", bool bold = false, bool wapText = false, bool border = true, string bgColor = null);
}
