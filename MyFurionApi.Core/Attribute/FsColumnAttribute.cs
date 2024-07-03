namespace MyFurionApi.Core;

/// <summary>
/// SqlSugar列属性
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class FsColumnAttribute : SqlSugar.SugarColumn
{
    /// <summary>
    /// 基本，只设置列说明，其他默认
    /// </summary>
    /// <param name="displayName">显示名</param>
    public FsColumnAttribute(string displayName)
    {
        base.ColumnDescription = displayName;
        base.IsNullable = true;
    }

    /// <summary>
    /// 基本，设置忽略
    /// </summary>
    /// <param name="isIgnore"></param>
    public FsColumnAttribute(bool isIgnore)
    {
        base.IsIgnore = isIgnore;
    }

    /// <summary>
    /// 通用，只设置列说明和是否可以为空
    /// </summary>
    /// <param name="displayName">列说明</param>
    /// <param name="isNullable">是否可以为空</param>
    public FsColumnAttribute(string displayName, bool isNullable)
    {
        base.ColumnDescription = displayName;
        base.IsNullable = isNullable;
    }

    /// <summary>
    /// 适用于String
    /// </summary>
    /// <param name="displayName">列说明</param>
    /// <param name="isNullable">是否可以为空</param>
    /// <param name="length">长度，默认255</param>
    public FsColumnAttribute(string displayName, bool isNullable, int length)
    {
        base.ColumnDescription = displayName;
        base.IsNullable = isNullable;
        base.Length = length;
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