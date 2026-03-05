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

    /// <summary>
    /// 模型数据
    /// </summary>
    public T Model { get; set; }

    /// <summary>
    /// 额外数据1
    /// </summary>
    public object Ext1 { get; set; }

    /// <summary>
    /// 额外数据2
    /// </summary>
    public object Ext2 { get; set; }

    /// <summary>
    /// 额外数据3
    /// </summary>
    public object Ext3 { get; set; }

    /// <summary>
    /// 额外数据4
    /// </summary>
    public object Ext4 { get; set; }

    /// <summary>
    /// 额外数据5
    /// </summary>
    public object Ext5 { get; set; }

    /// <summary>
    /// 额外数据6
    /// </summary>
    public object Ext6 { get; set; }

    /// <summary>
    /// 额外数据7
    /// </summary>
    public object Ext7 { get; set; }
}