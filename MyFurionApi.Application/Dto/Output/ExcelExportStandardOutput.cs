namespace MyFurionApi.Application.Dto;

/// <summary>
/// 导出标准输出
/// </summary>
/// <typeparam name="T"></typeparam>
public class ExcelExportStandardOutput<T>
{
    /// <summary>
    /// 列表数据
    /// </summary>
    public List<T> DataList { get; set; }
}