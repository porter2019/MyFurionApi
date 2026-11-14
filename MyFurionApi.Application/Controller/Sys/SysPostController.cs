namespace MyFurionApi.Application.Controller;

/// <summary>
/// 岗位管理
/// </summary>
[PermissionHandler("系统管理", "岗位管理", "SysPost", 0)]
public class SysPostController : BaseApiController
{
    private readonly ILogger<SysPostController> _logger;
    private readonly SqlSugarRepository<SysPost> _sysPostRepository;
    private readonly string _logPath = "系统设置 - 岗位管理";
    private readonly string _logHandler = "岗位信息";

    public SysPostController(ILogger<SysPostController> logger,
        SqlSugarRepository<SysPost> sysPostRepository)
    {
        _logger = logger;
        _sysPostRepository = sysPostRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist")]
    [Permission("查看", "show")]
    public async Task<SqlSugarPagedList<SysPost>> GetPageList(SysPostPageInput req)
    {
        var data = await _sysPostRepository.ToPageListAsync(req);
        return data;
    }

    /// <summary>
    /// 批量修改排序
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, Route("modify/sorts")]
    public async Task<string> ModifySorts(List<int> req)
    {
        if (req.IsEmpty()) throw Oops.Bah("缺少数据");

        var updateObjs = new List<SysPost>();
        for (var i = 0; i < req.Count; i++)
        {
            updateObjs.Add(new SysPost()
            {
                Id = req[i],
                Sort = i + 1
            });
        }
        await _sysPostRepository.EntityContext.Updateable<SysPost>(updateObjs).UpdateColumns(x => new { x.Sort }).ExecuteCommandAsync();
        return "保存成功";
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    public async Task<SysPost> GetInfo(int id)
    {
        var entity = await _sysPostRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) entity = new SysPost();
        return entity;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("add")]
    [Permission("添加", "add")]
    [UnitOfWork]
    public async Task<string> Add(SysPost req)
    {
        var id = await _sysPostRepository.InsertReturnIdentityAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
        return "添加成功";
    }

    /// <summary>
    /// 检查名称是否存在
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("check/name")]
    public async Task<bool> CheckNameExists(int id, string name)
    {
        return await _sysPostRepository.Entities.WhereIF(id > 0, x => x.Id != id).AnyAsync(x => x.Name.Equals(name));
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("edit"), Permission("修改", "edit")]
    [UnitOfWork]
    public async Task<string> Update(SysPost req)
    {
        await _sysPostRepository.UpdateAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

        return "修改成功";
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    [HttpDelete, UnitOfWork, Permission("删除", "delete")]
    public async Task<string> Delete(string ids)
    {
        var idList = ids.SplitWithComma().ConvertIntList();
        await _sysPostRepository.Change<SysPostUser>().DeleteAsync(x => idList.Contains(x.PostId));
        await _sysPostRepository.DeleteWithSoftAuditAsync(idList, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
        return "删除成功";
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("switch/status")]
    public async Task<string> ChangeStatus(int id)
    {
        var model = await _sysPostRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw Oops.Bah("数据不存在");
        var newStatus = !model.Status;
        var newStatusText = newStatus ? "已启用" : "已禁用";

        await _sysPostRepository.UpdateAuditAsync(x => new SysPost()
        {
            Status = newStatus,

            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = _logPath,
            ExtraHandler = _logHandler,
            ExtraInfo = $"更新后的状态：{newStatusText}",
            ClientType = CommonHelper.GetClientType(),
        });

        return newStatusText;
    }

    /// <summary>
    /// 自动搜索字段
    /// </summary>
    /// <param name="commonFuncService"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    [HttpGet, Route("auto/search/name")]
    public Task<List<dynamic>> SearchGroupField([FromServices] ICommonFuncService commonFuncService, string keyword)
    {
        return commonFuncService.AutoSearchField(typeof(SysPost), where: $"position('{keyword}' in Name) > 0");
    }
}

