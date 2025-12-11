using SqlSugar;

namespace MyFurionApi.Core;

/// <summary>
/// SqlSugar列属性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class FsColumnAttribute : SqlSugar.SugarColumn
{
    /// <summary>
    /// 基本，默认可空
    /// </summary>
    public FsColumnAttribute()
    {
        base.IsNullable = true;
    }

    /// <summary>
    /// 基本，忽略迁移
    /// </summary>
    /// <param name="isIgnore"></param>
    public FsColumnAttribute(bool isIgnore)
    {
        base.IsIgnore = isIgnore;
    }

    /// <summary>
    /// 适用于String
    /// </summary>
    /// <param name="length">长度，默认255，如果 = 9999，则使用string大文本</param>
    public FsColumnAttribute(int length)
    {
        if (length == 9999)
        {
            base.ColumnDataType = StaticConfig.CodeFirst_BigString;
        }
        else
        {
            base.Length = length;
        }
    }

    /// <summary>
    /// 必须这样写，不然内存溢出
    /// </summary>
    private bool _IsPK;

    /// <summary>
    /// 是否主键(为True时同时自增)
    /// </summary>
    public bool IsPK
    {
        get { return _IsPK; }
        set
        {
            _IsPK = value;

            base.IsPrimaryKey = _IsPK;
            base.IsIdentity = _IsPK;
        }
    }

    //private bool _VerRequired;

    ///// <summary>
    ///// 验证必填
    ///// </summary>
    //public bool VerRequired
    //{
    //    get { return _VerRequired; }
    //    set
    //    {
    //        _VerRequired = value;
    //    }
    //}

}