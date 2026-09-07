namespace MyFurionApi.Application.Dto;

/// <summary>
/// 导出选项基类
/// </summary>
public class BaseFormExportOption : BaseFormPostModel
{
    /// <summary>
    /// 格式，excel/pdf
    /// </summary>
    public string Format { get; set; }

}


/// <summary>
/// Get导出选项
/// </summary>
public class GetExportOptionInput : BaseFormExportOption
{
    /// <summary>
    /// 是否显示价格
    /// </summary>
    public bool ShowPrice { get; set; }
}

/// <summary>
/// Post导出选项
/// </summary>
public class PostExportOptionsInput : BaseFormExportOption
{
    /// <summary>
    /// 可以放PageList的请求参数，使用时显示转换一下
    /// </summary>
    public object QueryObject { get; set; }
}