namespace MyFurionApi.Application.Controller;

/// <summary>
/// 树型实体
/// </summary>
//[PermissionHandler("演示", "树", "Tree", 20)]
public class TreeController : BaseApiController
{
    private readonly ILogger<TreeController> _logger;
    private readonly SqlSugarRepository<Tree> _treeRepository;
    private readonly string _logPath = "原料管理 - 材料分类";
    private readonly string _logHandler = "材料分类";

    public TreeController(ILogger<TreeController> logger, SqlSugarRepository<Tree> treeRep)
    {
        _logger = logger;
        _treeRepository = treeRep;
    }

    /// <summary>
    /// 获取树列表
    /// </summary>
    /// <param name="search">搜索关键字</param>
    /// <returns></returns>
    [HttpGet, Route("get/tree")]
    [Permission("查看", "show")]
    public Task<List<Tree>> GetList(string search)
    {
        if (search.IsNull())
        {
            //如果没有搜索条件、或者搜索条件不会破坏树的构造，则可以直接加where
            return _treeRepository.Entities
                                  .Includes(x => x.Parent) //同时把父级信息也查出来
                                  .OrderBy(x => x.FullOrderNo)
                                  .ToTreeAsync(x => x.Childs, x => x.ParentId, 0);
        }
        else
        {
            //如果搜索条件破坏树的构造，如果直接where的话，因为可能找不到PId的数据，导致树不完整
            //1.先根据搜索出所有符合筛选条件的数据
            //2.再将搜索的id数据传入ToTree构造器中,ToTree会自动自动构造出缺少的父级数据，形成最终完整的树数据
            //这种会查两次数据库，第一次筛选条件，第二次查询所有的数据
            var ids = _treeRepository.Entities
                                             .Where(x => x.Name.Contains(search))
                                             .Select(x => x.Id)
                                             .ToList()
                                             .Cast<object>()
                                             .ToArray();
            return _treeRepository.Entities
                                  .OrderBy(x => x.FullOrderNo)
                                  .ToTreeAsync(x => x.Childs, x => x.ParentId, 0, ids);
        }
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    [Permission("查看", "show")]
    public async Task<Tree> GetInfo(int id)
    {
        var entity = await _treeRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            entity = new Tree();
        }

        return entity;
    }

    /// <summary>
    /// 生成排序号
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [HttpGet, Route("get/next/orderno")]
    public async Task<string> GetNextOrderNo(int pid)
    {
        int length = 3;
        var parentEntity = await _treeRepository.FirstOrDefaultAsync(p => p.Id == pid);
        var parentOrderNo = parentEntity?.OrderNo ?? "";
        var nowMaxEntity = await _treeRepository.FirstOrDefaultAsync(new TreeGenerateNextNoQuery() { ParentId = pid, OrderBy = "OrderNo DESC" });
        var lastOrderNo = "001";
        if (nowMaxEntity != null)
        {
            var nowOrderNo = nowMaxEntity.OrderNo.Substring(parentOrderNo.Length);
            var nowMaxOrderNo = (nowOrderNo.ObjToInt() + 1).ToString();
            lastOrderNo = nowMaxOrderNo.PadLeft(length, '0');
        }
        return parentOrderNo + lastOrderNo;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Permission("添加", "add")]
    public async Task<string> Add(Tree req)
    {
        await _treeRepository.InsertReturnIdentityAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

        //pgsql是函数
        await _treeRepository.Ado.ExecuteCommandAsync("SELECT fn_update_tree_layer();");
        //mysql是存储过程
        //await _treeRep.Ado.UseStoredProcedure().ExecuteCommandAsync("sp_update_tree_layer");

        return "添加成功";
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Permission("修改", "edit")]
    public async Task<string> Edit(Tree req)
    {
        await _treeRepository.UpdateAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

        await _treeRepository.Ado.ExecuteCommandAsync("SELECT fn_update_tree_layer();");

        return "修改成功";
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Permission("删除", "delete")]
    public async Task<string> Delete(int id)
    {
        var allChilds = await _treeRepository.AsQueryable().ToChildListAsync(x => x.ParentId, id);
        if (allChilds.Count < 1) return "删除的数据为空";
        await _treeRepository.DeleteAuditAsync(allChilds.Select(x => x.Id), new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
        return "删除成功";
    }
}
