using Furion.Logging.Extensions;
using SqlSugar;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace MyFurionApi.Core;

public static class SqlsugarSetup
{
    public static void AddSqlsugarSetup(this IServiceCollection services)
    {
        List<Type> types = App.EffectiveTypes.Where(a => !a.IsAbstract && a.IsClass && a.GetCustomAttributes(typeof(FsTableAttribute), true)?.FirstOrDefault() != null).ToList();
        SqlSugarScope sqlSugar = new SqlSugarScope(App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs"),
        db =>
        {
            //执行超时时间
            db.Ado.CommandTimeOut = 30;
            //Log
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                var sqlStr = UtilMethods.GetSqlString(db.Context.CurrentConnectionConfig.DbType, sql, pars);
                $"主库:\r\n {sqlStr} \r\n".LogDebug<SqlSugarLogTag>();
                App.PrintToMiniProfiler("SqlSugar", "Info", sqlStr);
            };
            //数据执行前
            db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                //新增
                if (entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    if (entityInfo.PropertyName == "CreatedUserId")
                        entityInfo.SetValue(GetUserId());
                    if (entityInfo.PropertyName == "CreatedUserName")
                        entityInfo.SetValue(GetUserName());
                    if (entityInfo.PropertyName == "CreatedTime")
                        entityInfo.SetValue(DateTime.Now);

                    if (entityInfo.PropertyName == "UpdatedUserId")
                        entityInfo.SetValue(GetUserId());
                    if (entityInfo.PropertyName == "UpdatedUserName")
                        entityInfo.SetValue(GetUserName());
                    if (entityInfo.PropertyName == "UpdatedTime")
                        entityInfo.SetValue(DateTime.Now);


                }

                // 更新操作
                if (entityInfo.OperationType == DataFilterType.UpdateByObject)
                {
                    if (entityInfo.PropertyName == "UpdatedTime")
                        entityInfo.SetValue(DateTime.Now);

                    if (entityInfo.PropertyName == "UpdatedUserId")
                        entityInfo.SetValue(GetUserId());
                    if (entityInfo.PropertyName == "UpdatedUserName")
                        entityInfo.SetValue(GetUserName());
                }
            };
            //全局过滤器
            foreach (var entityType in types)
            {
                //查询加上IsDeleted=0
                if (!entityType.GetProperty("IsDeleted").IsEmpty())
                {
                    //构建动态Lambda
                    var lambda = DynamicExpressionParser.ParseLambda
                    (new[] { Expression.Parameter(entityType, "it") },
                     typeof(bool), $"{nameof(BaseEntityStandard.IsDeleted)} ==  @0",
                      false);
                    db.QueryFilter.Add(new TableFilterItem<object>(entityType, lambda, true)); //将Lambda传入过滤器
                }
            }

        });
        services.AddSingleton<ISqlSugarClient>(sqlSugar);

        // 注册 SqlSugar 仓储
        services.AddScoped(typeof(SqlSugarRepository<>));

        //工作单元
        services.AddUnitOfWork<SqlSugarUnitOfWork>();
    }

    /// <summary>
    /// 获取当前用户id
    /// </summary>
    /// <returns></returns>
    private static object GetUserId()
    {
        if (App.User == null) return null;
        return App.User.FindFirst(ClaimConst.UserId)?.Value;
    }

    /// <summary>
    /// 获取当前用户名
    /// </summary>
    /// <returns></returns>
    private static object GetUserName()
    {
        if (App.User == null) return null;
        return App.User.FindFirst(ClaimConst.UserName)?.Value;
    }

}

public class SqlSugarLogTag { }