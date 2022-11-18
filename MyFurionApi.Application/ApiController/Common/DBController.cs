using System.Reflection;

namespace MyFurionApi.Application.ApiController;

/// <summary>
/// 数据库
/// </summary>
[AllowAnonymous]
public class DBController : BaseApiController
{
    private readonly ILogger<DBController> _logger;
    private readonly ISqlSugarClient _db;

    public DBController(ILogger<DBController> logger, ISqlSugarClient db)
    {
        _logger = logger;
        _db = db;
    }

    /// <summary>
    /// 数据库迁移
    /// </summary>
    /// <param name="tableNames">指定实体名称，英文逗号分割</param>
    /// <returns></returns>
    [HttpGet]
    public string Migration(string tableNames)
    {
        _logger.LogDebug("======数据库迁移开始======");
        _db.DbMaintenance.CreateDatabase();
        _logger.LogDebug($"数据库创建完成，如果是新建，注意修改数据库排序规则为：Chinese_PRC_CI_AS");
        //修改数据库排序规则为：Chinese_PRC_CI_AS  不然中文会乱码
        //ALTER DATABASE 数据库名称 SET SINGLE_USER WITH ROLLBACK IMMEDIATE
        //ALTER DATABASE 数据库名称 COLLATE Chinese_PRC_CI_AS
        //ALTER DATABASE 数据库名称 SET MULTI_USER

        var tableNameList = tableNames.SplitWithComma();
        List<Type> types = App.EffectiveTypes.Where(a => !a.IsAbstract && a.IsClass && a.GetCustomAttributes(typeof(SugarTable), true)?.FirstOrDefault() != null).ToList();
        foreach (var type in types)
        {
            //如果添加了禁止迁移属性，则跳过
            if (type.GetCustomAttribute(typeof(DisableSyncStructureAttribute), true) != null)
            {
                _logger.LogDebug($"已禁止迁移实体【{type.FullName}】");
                continue;
            }

            if (tableNameList.Any())
            {
                if (tableNameList.Contains(type.Name))
                {
                    _db.CodeFirst.InitTables(type);
                    _logger.LogDebug($"指定了实体名称【{type.Name}】，已迁移实体【{type.FullName}】");
                }
            }
            else
            {

                _db.CodeFirst.InitTables(type);
                _logger.LogDebug($"已迁移实体【{type.FullName}】");
            }
        }

        _logger.LogDebug("======数据库迁移结束======");
        return "OK";
    }

    /// <summary>
    /// 执行数据库脚本
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public string SqlScript()
    {
        //先刷新视图
        var refreshViewSql = "select TABLE_NAME from INFORMATION_SCHEMA.VIEWS order by TABLE_NAME";
        var viewNameList = _db.Ado.SqlQuery<string>(refreshViewSql);
        foreach (var viewName in viewNameList)
        {
            _db.Ado.ExecuteCommand($"exec sp_refreshview {viewName}");
            _logger.LogDebug($"刷新视图：{viewName}");
        }
        //获取启动项根目录 var webRootPath = App.HostEnvironment.ContentRootPath;
        //获取网站根目录wwwroot目录 var wwwroot = App.WebHostEnvironment.WebRootPath;

        var sqlScriptFolder = Path.Combine(App.WebHostEnvironment.WebRootPath, "content", "sqlscript");
        var sqlFileList = Directory.GetFiles(sqlScriptFolder, "*.sql", SearchOption.AllDirectories);
        foreach (var sqlFilePath in sqlFileList)
        {
            var sqlViewText = File.ReadAllText(sqlFilePath);
            if (sqlViewText.IsNull()) continue;

            var arrSqlView = sqlViewText.Split(new string[2]{
                                    "\r\ngo\r\n",
                                    "\r\nGO\r\n"
                                }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var sqlItem in arrSqlView)
            {
                if (string.IsNullOrEmpty(sqlItem)) continue;
                string execSql = sqlItem;
                try
                {
                    if (sqlItem.EndsWith("go") || sqlItem.EndsWith("GO"))
                    {
                        execSql = sqlItem.Substring(0, sqlItem.Length - 2);
                    }
                    _db.Ado.ExecuteCommand(execSql);
                }
                catch (Exception e)
                {
                    _logger.LogError($"执行Sql脚本文件出错，文件：{sqlFilePath}，错误信息：{e.Message}");
                    continue;
                }
            }
        }
        return "OK";
    }

}
