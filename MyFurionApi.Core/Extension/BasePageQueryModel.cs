using Furion.FriendlyException;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace MyFurionApi.Core;

/// <summary>
/// 所有分页控制器接口需要继承的基类
/// </summary>
public class BasePageQueryModel<TEntity> : BaseBuildWhereModel where TEntity : class, new()
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public PageOptions<TEntity> PageInfo { get; set; }

}

/// <summary>
/// 查询单条实体所需
/// </summary>
public class BaseSingleQueryModel : BaseBuildWhereModel
{
    private string _orderBy = "Id DESC";

    /// <summary>
    /// 排序，默默人Id DESC
    /// </summary>
    public string OrderBy
    {
        get
        {
            return string.IsNullOrEmpty(_orderBy) ? "Id DESC" : _orderBy;
        }
        set
        {
            _orderBy = value;
        }
    }
}

/// <summary>
/// List查询需要继承的基类
/// </summary>
public class BaseListQueryModel : BaseBuildWhereModel
{
    /// <summary>
    /// 前几条，如果为0，则条件不生效
    /// </summary>
    public int Top { get; set; }


    private string _orderBy = "Id DESC";

    /// <summary>
    /// 排序，默默人Id DESC
    /// </summary>
    public string OrderBy
    {
        get
        {
            return string.IsNullOrEmpty(_orderBy) ? "Id DESC" : _orderBy;
        }
        set
        {
            _orderBy = value;
        }
    }

}

/// <summary>
/// 所有需要自动构建where条件的请求实体需要继承
/// </summary> 
public class BaseBuildWhereModel : BaseFormPostModel
{
    /// <summary>
    /// 创建分页查询的where条件
    /// </summary>
    /// <returns></returns>
    public virtual string BuildPageSearchWhere()
    {
        var currentType = this.GetType();

        StringBuilder sbWhere = new();

        foreach (var itemType in currentType.GetProperties())
        {
            var quarrAttrArray = itemType.GetCustomAttributes(typeof(PageQueryAttribute), true);
            if (!quarrAttrArray.Any()) continue;
            var queryAttr = quarrAttrArray[0] as PageQueryAttribute;
            if (queryAttr.IsIgnore) continue;

            var dbType = queryAttr.DbType;
            var filedName = itemType.Name;
            var filedValue = itemType.GetValue(this, null);
            if (filedValue == null) continue;
            if (filedValue.ToString().IsNull()) continue;
            var fileValueChar = "";
            // sql server使用[]包含列名，mysql使用``，postgresql使用``，sqlite使用[]
            var columnName = string.Empty;

            if (dbType == SqlSugar.DbType.MySql)
            {
                columnName = queryAttr.ColumnName.IsNull() ? $"`{filedName}`" : $"`{queryAttr.ColumnName}`";
            }
            else if (dbType == SqlSugar.DbType.PostgreSQL)
            {
                columnName = queryAttr.ColumnName.IsNull() ? $"\"{filedName}\"" : $"\"{queryAttr.ColumnName}\"";
            }
            else if (dbType == SqlSugar.DbType.SqlServer)
            {
                columnName = queryAttr.ColumnName.IsNull() ? $"[{filedName}]" : $"[{queryAttr.ColumnName}]";
            }
            else if (dbType == SqlSugar.DbType.Sqlite)
            {
                columnName = queryAttr.ColumnName.IsNull() ? $"[{filedName}]" : $"[{queryAttr.ColumnName}]";
            }

            var sqlColumnName = queryAttr.PrefixName.IsNull() ? columnName : queryAttr.PrefixName + "." + columnName;

            var propType = itemType.PropertyType;
            if (propType == typeof(System.String))
            {
                fileValueChar = "'";
            }
            else if (propType == typeof(System.Boolean) || (propType == typeof(System.Nullable<Boolean>)))
            {
                filedValue = filedValue.ToString().EqualsIgnoreCase("true") ? 1 : 0;
            }
            var logic = queryAttr.Logic.GetEnumDescription();
            switch (queryAttr.Operator)
            {
                case PageQueryOperatorType.Equal:
                    sbWhere.Append($" {logic} {sqlColumnName} = {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.NotEqual:
                    sbWhere.Append($" {logic} {sqlColumnName} != {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.GreaterThan:
                    sbWhere.Append($" {logic} {sqlColumnName} > {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.GreaterThanOrEqual:
                    sbWhere.Append($" {logic} {sqlColumnName} >= {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.LessThan:
                    sbWhere.Append($" {logic} {sqlColumnName} < {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.LessThanOrEqual:
                    sbWhere.Append($" {logic} {sqlColumnName} <= {fileValueChar}{filedValue}{fileValueChar}");
                    break;

                case PageQueryOperatorType.BoolWhenTrue:
                    if (filedValue.ObjToInt() == 1)
                    {
                        sbWhere.Append($" {logic} {sqlColumnName} = 1");
                    }
                    break;

                case PageQueryOperatorType.IntEqualWhenGreaterZero:
                    if (filedValue.ObjToInt() > 0)
                    {
                        sbWhere.Append($" {logic} {sqlColumnName} = {filedValue}");
                    }
                    break;

                case PageQueryOperatorType.IntEqualWhenGreaterMinus:
                    if (filedValue.ObjToInt() > -1)
                    {
                        sbWhere.Append($" {logic} {sqlColumnName} = {filedValue}");
                    }
                    break;

                case PageQueryOperatorType.Like:
                    sbWhere.Append($" {logic} {sqlColumnName} like '%{filedValue}%'");
                    break;

                case PageQueryOperatorType.LikeLeft:
                    sbWhere.Append($" {logic} {sqlColumnName} like '{filedValue}%'");
                    break;

                case PageQueryOperatorType.LikeRight:
                    sbWhere.Append($" {logic} {sqlColumnName} like '%{filedValue}'");
                    break;

                case PageQueryOperatorType.CharIndex:
                    if (dbType == SqlSugar.DbType.MySql)
                        sbWhere.Append($" {logic} locate('{filedValue}',{sqlColumnName}) > 0");
                    else if (dbType == SqlSugar.DbType.SqlServer)
                        sbWhere.Append($" {logic} CharIndex('{filedValue}',{sqlColumnName}) > 0");
                    else if (dbType == SqlSugar.DbType.PostgreSQL)
                        sbWhere.Append($" {logic} position('{filedValue}' in {sqlColumnName}) > 0");
                    else if ((dbType == SqlSugar.DbType.Sqlite))
                        sbWhere.Append($" {logic} instr({sqlColumnName},'{filedValue}') > 0");
                    else
                        throw Oops.Oh("CharIndex条件仅支持SqlServer、MySql、PostgreSQL、Sqlite数据库");
                    break;

                case PageQueryOperatorType.BetweenNumber:
                    var tempArr = filedValue.ToString().SplitWithSemicolon();
                    if (tempArr.Length != 2) throw Oops.Oh("Between条件下值的格式必须使用英文分号分割");
                    sbWhere.Append($" {logic} {sqlColumnName} between {tempArr[0]} and {tempArr[1]}");
                    break;

                case PageQueryOperatorType.BetweenDate:
                    var tempArr2 = filedValue.ToString().SplitWithSemicolon();
                    if (tempArr2.Length != 2) throw Oops.Oh("Between条件下值的格式必须使用英文分号分割");
                    sbWhere.Append($" {logic} {sqlColumnName} between '{tempArr2[0]}' and '{tempArr2[1]} 23:59:59'");
                    break;

                case PageQueryOperatorType.IntIn:
                    sbWhere.Append($" {logic} {sqlColumnName} in ({filedValue})");
                    break;

                case PageQueryOperatorType.IntNotIn:
                    sbWhere.Append($" {logic} {sqlColumnName} not in({filedValue})");
                    break;
                case PageQueryOperatorType.IsNull:
                    sbWhere.Append($" {logic} {sqlColumnName} is null ");
                    break;
                case PageQueryOperatorType.IsNotNULL:
                    sbWhere.Append($" {logic} {sqlColumnName} is not null ");
                    break;
                case PageQueryOperatorType.Sql:
                    sbWhere.Append($" {logic} {filedValue}");
                    break;
                default:
                    break;
            }
        }

        return sbWhere.ToString();
    }
}


/// <summary>
/// 分页条件
/// </summary>
public class PageOptions<TEntity> //where TEntity : class//Model.BaseEntity
{
    private int _pageIndex = 1;

    /// <summary>
    /// 当前页数
    /// </summary>
    public int PageIndex
    {
        get
        {
            return _pageIndex < 1 ? 1 : _pageIndex;
        }
        set
        {
            _pageIndex = value;
        }
    }

    private int _pageSize = 10;

    /// <summary>
    /// 每页数据量
    /// </summary>
    public int PageSize
    {
        get
        {
            return _pageSize < 1 ? 10 : _pageSize;
        }
        set
        {
            _pageSize = value;
        }
    }

    private string _where { get; set; }

    /// <summary>
    /// 查询条件
    /// </summary>
    [JsonIgnore]
    public string Where
    {
        get
        {
            return _where.IsNull() ? "" : _where;
        }
        set
        {
            _where = value.Trim();
            if (_where.IsNotNull())
            {
                if (!_where.ToLower().StartsWith("and ")) _where = "and " + _where;
            }
        }
    }

    private string _orderBy;

    /// <summary>
    /// 排序条件
    /// </summary>
    public string OrderBy
    {
        get
        {
            //如果没有设置排序
            if (_orderBy.IsNull())
            {
                return "Id DESC";
            }
            else
            {
                return _orderBy;
            }
        }
        set
        {
            _orderBy = value;
        }
    }
}


/// <summary>
/// 所有Post对象需要继承的基类
/// <code>方便在filter中自动注入当前登录的用户信息，这些信息在业务层中可能会需要用到</code>
/// </summary>
public class BaseFormPostModel
{
    /// <summary>
    /// 当前操作的用户id
    /// </summary>
    [JsonIgnore]
    public int CurrentUserId { get; set; }

    /// <summary>
    /// 当前操作的用户名
    /// </summary>
    [JsonIgnore]
    public string CurrentUserName { get; set; }

    /// <summary>
    /// 数据Id
    /// </summary>
    [SwaggerIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    [SwaggerIgnore]
    public long Version { get; set; }
}
