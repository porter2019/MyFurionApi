namespace MyFurionApi.Application;

/// <summary>
/// 通用的函数服务
/// </summary>
public class CommonFuncService : ICommonFuncService, ITransient
{
    private readonly ILogger<CommonFuncService> _logger;
    private readonly SqlSugarRepository<SysUser> _dbRepository;

    private static readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

    public CommonFuncService(ILogger<CommonFuncService> logger, SqlSugarRepository<SysUser> dbRepository)
    {
        _logger = logger;
        _dbRepository = dbRepository;
    }

    /// <summary>
    /// 生成下一个编号
    /// </summary>
    /// <param name="entityType">实体对象，内部会查找对应的表名</param>
    /// <param name="columnName">数据中Code部分列名</param>
    /// <param name="codeLength">编号部分长度，比如001、002这种就传入3，0001、0002就传入4</param>
    /// <param name="extWhere">额外的sql where条件： and OrderType = 1 </param>
    /// <param name="isToDay">是否今天的</param>
    /// <returns>001、002、003</returns>
    public async Task<string> GenerateNextCodeAsync(Type entityType, string columnName, int codeLength, string extWhere, bool isToDay = true)
    {
        try
        {
            var todayWhere = string.Empty;

            var dbType = _dbRepository.EntityContext.CurrentConnectionConfig.DbType;
            if (dbType == DbType.MySql)
                todayWhere = " and date(CreatedTime)  = curdate()";
            else if (dbType == DbType.PostgreSQL)
                todayWhere = " and date(CreatedTime)  = current_date";
            else if (dbType == DbType.SqlServer)
                todayWhere = " and DATEDIFF(day, CreatedTime, GETDATE()) = 0";
            else if (dbType == DbType.Sqlite)
                todayWhere = " and date(CreatedTime)  = date('now')";
            else
                throw Oops.Oh("暂不支持该数据库类型");
            await _mutex.WaitAsync();

            // 获取表名
            var tableName = _dbRepository.Context.EntityMaintenance.GetEntityInfo(entityType).DbTableName;

            var sql = $"select {columnName} from {tableName} where 1=1 {todayWhere} {extWhere} order by \"Id\" desc limit 1;";
            var maxCode = await _dbRepository.Ado.SqlQuerySingleAsync<string>(sql);
            var code = 0;
            if (maxCode.IsNotNull())
            {
                code = maxCode.Substring(maxCode.Length - codeLength).ObjToInt();
            }
            code++;
            return code.ToString().PadLeft(codeLength, '0');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成下一个编号失败");
            return string.Empty;
        }
        finally
        {
            _mutex.Release();
        }
    }

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
    public Task<List<dynamic>> AutoSearchField(Type entityType, string where = null, string idCoulumnName = "Id", string nameColumnName = "Name", bool excludeDeleted = true, int limit = 10)
    {
        // 获取表名
        var tableName = _dbRepository.Context.EntityMaintenance.GetEntityInfo(entityType).DbTableName;

        var whereSql = " 1=1 ";
        if (where.IsNotNull()) whereSql += " and " + where;
        if (excludeDeleted) whereSql += " and IsDeleted = 0 ";

        var sql = $"select {idCoulumnName} as Id,{nameColumnName} as Name from {tableName} where {whereSql} limit {limit}";

        return _dbRepository.Ado.SqlQueryAsync<dynamic>(sql);
    }

}
