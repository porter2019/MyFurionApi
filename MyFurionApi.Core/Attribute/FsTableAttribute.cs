namespace MyFurionApi.Core;

/// <summary>
/// SqlSugar表属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class FsTableAttribute : SqlSugar.SugarTable
{
    /// <summary>
    /// 默认
    /// </summary>
    public FsTableAttribute()
    {

    }

    /// <summary>
    /// 指定表名
    /// </summary>
    /// <param name="tableName">表名</param>
    public FsTableAttribute(string tableName)
    {
        base.TableName = tableName;
    }

    /// <summary>
    /// 忽略迁移
    /// </summary>
    /// <param name="isIgnore">是否忽略迁移</param>
    public FsTableAttribute(bool isIgnore)
    {
        this.IsIgnore = isIgnore;
    }

    /// <summary>
    /// 指定表名和是否忽略，一般视图类使用
    /// </summary>
    /// <param name="tableName">表名</param>
    /// <param name="isIgnore">是否忽略迁移</param>
    public FsTableAttribute(string tableName, bool isIgnore)
    {
        base.TableName = tableName;
        this.IsIgnore = isIgnore;
    }


    /// <summary>
    /// 必须这样写，不然内存溢出
    /// </summary>
    private bool _IsIgnore = false;

    /// <summary>
    /// 是否主键(为True时同时自增)
    /// </summary>
    public bool IsIgnore
    {
        get { return _IsIgnore; }
        set
        {
            _IsIgnore = value;
        }
    }
}
