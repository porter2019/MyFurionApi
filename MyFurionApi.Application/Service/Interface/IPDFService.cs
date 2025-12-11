namespace MyFurionApi.Application;

public interface IPDFService
{
    Task<bool> ExcelToPdf(string excelPath, string pdfPath);
}
