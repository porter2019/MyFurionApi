using Furion.EventBus;
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
            };
            //数据执行前
            db.Aop.DataExecuting = (oldValue, entityInfo) =>
            {
                //新增
                if (entityInfo.OperationType == DataFilterType.InsertByObject)
                {
                    if (entityInfo.PropertyName == "CreatedUserId" && (oldValue == null || oldValue == DBNull.Value)) //判断为null的情况是事件订阅中的添加操作
                        entityInfo.SetValue(GetUserId());
                    if (entityInfo.PropertyName == "CreatedUserName" && (oldValue == null || (oldValue is string str && str == "") || oldValue == DBNull.Value))
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
            //审计日志
            db.Aop.OnDiffLogEvent = it =>
            {
                var customData = (LogAction)it.BusinessData;
                var diffType = it.DiffType;
                var timeCostMs = (it.Time?.TotalMilliseconds ?? 0) + " ms";

                $"审计：操作类型：{diffType} \r\n 耗时:{timeCostMs} \r\n BeforeData：{JsonHelper.Serialize(it.BeforeData)}\r\n AfterData：{JsonHelper.Serialize(it.AfterData)}\r\n Sql：{it.Sql} \r\n parameter：{JsonHelper.Serialize(it.Parameters)} \r\n 自定义数据 ：{JsonHelper.Serialize(customData)}".LogDebug<SqlSugarLogTag>();

                var buildInfo = BuildAuditDescription(it);

                var sb = new StringBuilder();
                sb.AppendLine(buildInfo.Item1.GetEnumDescription());
                sb.AppendLine(customData.ExtraHandler + "；");
                if (customData.ExtraInfo.IsNotNull()) sb.AppendLine(customData.ExtraInfo + "；");
                sb.AppendLine(buildInfo.Item3 + "；");
                if (buildInfo.Item2.IsNotNull()) sb.AppendLine($"Id：{buildInfo.Item2}" + "；");
                sb.AppendLine($"Time：{timeCostMs}");

                customData.Type = buildInfo.Item1;
                customData.Content = sb.ToString();
                customData.CreatedTime = DateTime.Now;
                if (customData.CreatedUserId == null) customData.CreatedUserId = GetUserId().ObjToInt();
                if (customData.CreatedUserName.IsNull()) customData.CreatedUserName = GetUserName();

                var publisher = App.GetService<IEventPublisher>();
                publisher.PublishAsync("Log:Action:Add", customData);

            };

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
    private static string GetUserName()
    {
        if (App.User == null) return null;
        return App.User.FindFirst(ClaimConst.UserName)?.Value.ToString();
    }

    /// <summary>
    /// 构建审计日志描述
    /// </summary>
    static Tuple<LogActionType, string, string> BuildAuditDescription(DiffLogModel diff)
    {
        List<string> ignoreColumns = ["CreatedTime", "UpdatedTime", "IsDeleted", "CreatedUserId", "CreatedUserName", "UpdatedUserId", "UpdatedUserName", "IsDeleted"];

        string mainIds = null;
        var beforeData = diff.BeforeData;
        var afterData = diff.AfterData;

        var sourceData = beforeData ?? afterData;
        if (sourceData != null && sourceData.Any())
        {
            var ids = new List<string>();

            foreach (var row in sourceData)
            {
                var keyCol = row.Columns.FirstOrDefault(p => p.IsPrimaryKey == true);
                if (keyCol?.Value != null)
                {
                    var val = keyCol.Value.ToString();
                    if (val != "0")
                        ids.Add(keyCol.Value.ToString());
                }
            }

            if (ids.Count > 0)
                mainIds = string.Join(",", ids.Distinct());
        }

        var sb = new StringBuilder();

        //1. 软删除
        if (diff.Sql.IsNotNull() && diff.Sql.Contains("`IsDeleted`=@Const0") && beforeData != null && diff.DiffType == DiffType.update)
        {
            foreach (var beforeTable in beforeData)
            {
                var list = beforeTable.Columns
                    .Where(c => !ignoreColumns.Contains(c.ColumnName))
                    .Select(c => $"{c.ColumnDescription}({c.ColumnName})：{c.Value}")
                    .ToList();

                sb.AppendLine($"{string.Join("，", list)} {beforeTable.TableName}({beforeTable.TableDescription})");
            }
            return new Tuple<LogActionType, string, string>(LogActionType.删除, mainIds, sb.ToString());
        }

        //2. 新增
        if (afterData != null && diff.DiffType == DiffType.insert)
        {
            foreach (var afterTable in afterData)
            {
                var list = afterTable.Columns
                    .Where(c => !ignoreColumns.Contains(c.ColumnName))
                    .Select(c => $"{c.ColumnDescription}({c.ColumnName})：{c.Value}")
                    .ToList();

                sb.AppendLine($"{string.Join("，", list)}  {afterTable.TableName}({afterTable.TableDescription})");
            }
            return new Tuple<LogActionType, string, string>(LogActionType.新增, mainIds, sb.ToString());
        }

        // 3. 修改
        if (beforeData != null && afterData != null && diff.DiffType == DiffType.update)
        {
            foreach (var afterTable in afterData)
            {
                var beforeTable = beforeData.FirstOrDefault(x => x.TableName == afterTable.TableName);
                if (beforeTable == null)
                    continue;

                var changes = new List<string>();
                foreach (var afterCol in afterTable.Columns)
                {
                    if (ignoreColumns.Contains(afterCol.ColumnName))
                        continue;

                    var beforeCol = beforeTable.Columns.FirstOrDefault(c => c.ColumnName == afterCol.ColumnName);
                    if (beforeCol == null)
                        continue;

                    var oldVal = beforeCol.Value?.ToString();
                    var newVal = afterCol.Value?.ToString();

                    if (oldVal != newVal)
                        changes.Add($"{afterCol.ColumnDescription}({afterCol.ColumnName})：{oldVal} → {newVal}");
                }

                if (changes.Count > 0)
                    sb.AppendLine($"{string.Join("，", changes)} {beforeTable.TableName}({beforeTable.TableDescription})");
            }

            if (sb.Length == 0)
                sb.AppendLine("无字段变化");

            return new Tuple<LogActionType, string, string>(LogActionType.修改, mainIds, sb.ToString());
        }

        // 4. 物理删除
        if (beforeData != null && diff.DiffType == DiffType.delete)
        {
            List<string> CoreColumns = ["Id", "IsDeleted"];
            foreach (var beforeTable in beforeData)
            {
                var list = beforeTable.Columns
                    .Where(c => !CoreColumns.Contains(c.ColumnName))
                    .Select(c => $"{c.ColumnDescription}({c.ColumnName})：{c.Value}")
                    .ToList();

                sb.AppendLine($"{string.Join("，", list)} {beforeTable.TableName}({beforeTable.TableDescription})");
            }

            return new Tuple<LogActionType, string, string>(LogActionType.删除, mainIds, sb.ToString());
        }

        // 未识别的操作
        return new Tuple<LogActionType, string, string>(LogActionType.新增, mainIds, "未知操作");
    }

}

public class SqlSugarLogTag { }