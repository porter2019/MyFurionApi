
namespace MyFurionApi.Application;
public interface ICommonFuncService
{
    /// <summary>
    /// 自动搜索字段
    /// </summary>
    /// <param name="entityType">实体名称，自动查找表名</param>
    /// <param name="where">条件语句：mysql：locate('{keyword}',CarPlateNum) > 0  pgsql：position('001' in Code) > 0</param>
    /// <param name="idCoulumnName">Id在数据库中的字段名称</param>
    /// <param name="nameColumnName">Name在数据库中的字段名称</param>
    /// <param name="excludeDeleted">是否排除已删除的数据，默认true</param>
    /// <param name="limit">最多返回多少条数据，默认10条</param>
    /// <returns>统一返回Id，Name这两个字段的列表</returns>
    Task<List<dynamic>> AutoSearchField(Type entityType, string where = null, string idCoulumnName = "Id", string nameColumnName = "Name", bool excludeDeleted = true, int limit = 10);

    /// <summary>
    /// 生成今日下一个编号,按照每天递增
    /// </summary>
    /// <param name="entityType">实体对象，内部会查找对应的表名</param>
    /// <param name="columnName">数据中Code部分列名</param>
    /// <param name="codeLength">编号部分长度，比如001、002这种就传入3，0001、0002就传入4</param>
    /// <param name="extWhere">额外的sql where条件： and OrderType = 1 </param>
    /// <param name="isToDay">是否是今天的</param>
    /// <returns>001、002、003</returns>
    Task<string> GenerateNextCodeAsync(Type entityType, string columnName, int codeLength, string extWhere, bool isToDay = true);
}
