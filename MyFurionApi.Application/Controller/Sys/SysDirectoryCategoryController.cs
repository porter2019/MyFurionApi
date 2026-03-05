namespace MyFurionApi.Application.Controller;

/// <summary>
/// 字典分类管理
/// </summary>
//[PermissionHandler("系统设置", "字典管理", "SysDirectoryCategory", 0)]
public class SysDirectoryCategoryController : BaseApiController
{
    private readonly ILogger<SysDirectoryCategoryController> _logger;
    private readonly SqlSugarRepository<SysDirectoryCategory> _categoryRepository;

    public SysDirectoryCategoryController(ILogger<SysDirectoryCategoryController> logger, SqlSugarRepository<SysDirectoryCategory> categoryRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// 获取树列表
    /// </summary>
    /// <param name="search">搜索关键字</param>
    /// <returns></returns>
    [HttpGet, Route("get/tree")]
    [Permission("查看", "show")]
    public Task<List<SysDirectoryCategory>> GetList(string search)
    {
        //var tt = _categoryRepository.Context.EntityMaintenance.GetEntityInfo(typeof(SysDirectoryCategory)).DbTableName;
        if (search.IsNull())
        {
            //如果没有搜索条件、或者搜索条件不会破坏树的构造，则可以直接加where
            return _categoryRepository.Entities
                                  .Includes(x => x.Parent) //同时把父级信息也查出来
                                                           //.OrderBy(x => x.FullOrderNo)
                                  .ToTreeAsync(x => x.Childs, x => x.ParentId, 0);
        }
        else
        {
            //如果搜索条件破坏树的构造，如果直接where的话，因为可能找不到PId的数据，导致树不完整
            //1.先根据搜索出所有符合筛选条件的数据
            //2.再将搜索的id数据传入ToSysDirectoryCategory构造器中,ToSysDirectoryCategory会自动自动构造出缺少的父级数据，形成最终完整的树数据
            //这种会查两次数据库，第一次筛选条件，第二次查询所有的数据
            var ids = _categoryRepository.Entities
                                             .Where(x => x.Name.Contains(search))
                                             .Select(x => x.Id)
                                             .ToList()
                                             .Cast<object>()
                                             .ToArray();
            return _categoryRepository.Entities
                                  //.OrderBy(x => x.FullOrderNo)
                                  .ToTreeAsync(x => x.Childs, x => x.ParentId, 0, ids);
        }
    }

    /// <summary>
    /// 根据Code获取字典下面的项
    /// </summary>
    /// <param name="codes">多个英文逗号分割</param>
    /// <returns></returns>
    [HttpGet, Route("get/item/by/code")]
    public Task<List<SysDirectoryItem>> GetItemListByCode(string codes)
    {
        var codeArr = codes.SplitWithComma();
        return _categoryRepository.Change<SysDirectoryItem>().AsQueryable()
                    .FullJoin<SysDirectoryCategory>((i, c) => i.CategoryId == c.Id)
                    .Where((i, c) => codeArr.Contains(c.Code) && i.Status)
                    .ToListAsync();
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    [Permission("查看", "show")]
    public async Task<SysDirectoryCategory> GetInfo(int id)
    {
        var entity = await _categoryRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
        {
            entity = new SysDirectoryCategory();
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
        var parentEntity = await _categoryRepository.FirstOrDefaultAsync(p => p.Id == pid);
        var parentOrderNo = parentEntity?.OrderNo ?? "";
        var nowMaxEntity = await _categoryRepository.FirstOrDefaultAsync(new TreeGenerateNextNoQuery() { ParentId = pid, OrderBy = "OrderNo DESC" });
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
    /// 检查编码是否存在
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("check/code")]
    public async Task<bool> CheckNameExists(int id, string code)
    {
        return await _categoryRepository.Entities.WhereIF(id > 0, x => x.Id != id).AnyAsync(x => x.Code.Equals(code));
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Permission("添加", "add")]
    public async Task<string> Add(SysDirectoryCategory req)
    {
        await _categoryRepository.InsertReturnIdentityAuditAsync(req, new LogAction()
        {
            Local = "系统设置 - 字典管理",
            ExtraHandler = "字典分类",
            ClientType = CommonHelper.GetClientType(),
        });

        //pgsql是函数
        await _categoryRepository.Ado.ExecuteCommandAsync("SELECT fn_update_directory_category_layer()");
        //await _categoryRepository.Ado.ExecuteCommandAsync("SELECT fn_update_directory_category_layer(@tenant_id)", new { tenant_id = CurrentTenantId });

        return "添加成功";
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Permission("修改", "edit")]
    public async Task<string> Edit(SysDirectoryCategory req)
    {
        await _categoryRepository.UpdateAuditAsync(req, new LogAction()
        {
            Local = "系统设置 - 字典管理",
            ExtraHandler = "字典分类",
            ClientType = CommonHelper.GetClientType(),
        });
        await _categoryRepository.Ado.ExecuteCommandAsync("SELECT fn_update_directory_category_layer()");
        //await _categoryRepository.Ado.ExecuteCommandAsync("SELECT fn_update_directory_category_layer(@tenant_id)", new { tenant_id = CurrentTenantId });

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
        var allChilds = await _categoryRepository.AsQueryable().ToChildListAsync(x => x.ParentId, id);
        if (allChilds.Count < 1) return "删除的数据为空";
        await _categoryRepository.DeleteAuditAsync(allChilds.Select(x => x.Id), new LogAction()
        {
            Local = "系统设置 - 字典管理",
            ExtraHandler = "字典分类",
            ClientType = CommonHelper.GetClientType(),
        });
        return "删除成功";
    }
}
