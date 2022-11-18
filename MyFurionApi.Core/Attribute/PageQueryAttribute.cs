using System.ComponentModel;

namespace MyFurionApi.Core;

/// <summary>
/// 分页属性设置
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class PageQueryAttribute : Attribute
{
    public PageQueryAttribute()
    { }

    /// <summary>
    /// 只指定匹配类型
    /// </summary>
    /// <param name="action"></param>
    public PageQueryAttribute(PageQueryOperatorType action)
    {
        this.Operator = action;
    }

    /// <summary>
    /// 忽略
    /// </summary>
    public bool IsIgnore { get; set; } = false;

    /// <summary>
    /// 列前缀，a.
    /// </summary>
    public string PrefixName { get; set; }

    /// <summary>
    /// 对应数据库的字段名称，如果为空，则跟列名一样
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// 逻辑 And/Or，默认And
    /// </summary>
    public PageQueryLogicType Logic { get; set; } = PageQueryLogicType.And;

    /// <summary>
    /// 匹配类型
    /// </summary>
    public PageQueryOperatorType Operator { get; set; }
}

/// <summary>
/// where条件字段匹配类型
/// </summary>
public enum PageQueryOperatorType
{
    /// <summary>
    /// 等于
    /// </summary>
    Equal,

    /// <summary>
    /// 不等于
    /// </summary>
    NotEqual,

    /// <summary>
    /// 大于
    /// </summary>
    GreaterThan,

    /// <summary>
    /// 大于等于
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// 小于
    /// </summary>
    LessThan,

    /// <summary>
    /// 小于等于
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// bool类型使用，为True时给条件“=1”
    /// </summary>
    BoolWhenTrue,

    /// <summary>
    /// Int类型使用，当>0时给条件
    /// </summary>
    IntEqualWhenGreaterZero,

    /// <summary>
    /// Int类型使用，当>-1时给条件
    /// </summary>
    IntEqualWhenGreaterMinus,

    /// <summary>
    /// like '%参数%'
    /// </summary>
    Like,

    /// <summary>
    /// like '参数%'
    /// </summary>
    LikeLeft,

    /// <summary>
    /// like '%参数'
    /// </summary>
    LikeRight,

    /// <summary>
    /// CHARINDEX('参数',UserName) > 0
    /// </summary>
    CharIndex,

    /// <summary>
    /// 值必须使用;分割,between and
    /// </summary>
    BetweenNumber,

    /// <summary>
    /// 时间Between;分割,between and
    /// </summary>
    BetweenDate,

    /// <summary>
    /// Int类型的in查询，值必须英文逗号分隔
    /// </summary>
    IntIn,

    /// <summary>
    /// Int类型的not in查询，值必须英文逗号分割
    /// </summary>
    IntNotIn,

    /// <summary>
    /// 指定SQL语句查询: 1=1
    /// </summary>
    Sql,
}

/// <summary>
/// 逻辑方式
/// </summary>
public enum PageQueryLogicType
{
    /// <summary>
    /// and
    /// </summary>
    [Description("AND")]
    And,

    [Description("OR")]
    Or
}
