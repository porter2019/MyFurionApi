namespace MyFurionApi.Application.Controller;

/// <summary>
/// 部门管理
/// </summary>
[PermissionHandler("系统设置", "部门管理", "SysOrg", 0)]
public class SysOrgController : BaseApiController
{
    private readonly ILogger<SysOrgController> _logger;
    private readonly SqlSugarRepository<SysOrg> _treeRepository;
    private readonly string _logPath = "系统设置 - 部门管理";
    private readonly string _logHandler = "部门管理";

    public SysOrgController(ILogger<SysOrgController> logger, SqlSugarRepository<SysOrg> treeRep)
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
    public Task<List<SysOrg>> GetList(string search)
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
            //2.再将搜索的id数据传入ToSysOrg构造器中,ToSysOrg会自动自动构造出缺少的父级数据，形成最终完整的树数据
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
    /// 修改部门经理
    /// </summary>
    /// <param name="id"></param>
    /// <param name="leaderId"></param>
    /// <returns></returns>
    [HttpGet, Route("modify/learer")]
    public async Task<string> ModifyLeader(int id, int leaderId)
    {
        var orgModel = await _treeRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw Oops.Bah("部门不存在");
        var userModel = await _treeRepository.Change<SysUser>().FirstOrDefaultAsync(x => x.Id == leaderId) ?? throw Oops.Bah("用户不存在");

        await _treeRepository.UpdateAuditAsync(x => new SysOrg()
        {
            LeaderId = userModel.Id,
            LeaderName = userModel.UserName,
        }, x => x.Id == id, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
        return "修改成功";
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    [Permission("查看", "show")]
    public async Task<SysOrg> GetInfo(int id)
    {
        var entity = await _treeRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            entity = new SysOrg();
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
    public async Task<string> Add(SysOrg req)
    {
        await _treeRepository.InsertReturnIdentityAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

        //pgsql是函数
        await _treeRepository.Ado.ExecuteCommandAsync("SELECT fn_update_sys_org_layer();");
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
    public async Task<string> Edit(SysOrg req)
    {
        await _treeRepository.UpdateAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

        await _treeRepository.Ado.ExecuteCommandAsync("SELECT fn_update_sys_org_layer();");

        //修改用户中冗余存储的部门名称
        await _treeRepository.Change<SysUser>().UpdateAsync(x => new SysUser()
        {
            OrgName = req.Name
        }, x => x.OrgId == req.Id);

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
        //修改用户中冗余存储的部门名称
        await _treeRepository.Change<SysUser>().UpdateAsync(x => new SysUser()
        {
            OrgId = 0,
            OrgName = null,
        }, x => x.OrgId == id);
        return "删除成功";
    }
}
