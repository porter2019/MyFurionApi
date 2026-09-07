namespace MyFurionApi.Application.Controller;

/// <summary>
/// 字典项管理
/// </summary>
public class SysDirectoryItemController : BaseApiController
{
    private readonly ILogger<SysDirectoryItemController> _logger;
    private readonly SqlSugarRepository<SysDirectoryItem> _sysDirectoryItemRepository;
    private readonly string _logPath = "系统设置 - 字典管理";
    private readonly string _logHandler = "字典信息";

    public SysDirectoryItemController(ILogger<SysDirectoryItemController> logger,
        SqlSugarRepository<SysDirectoryItem> sysDirectoryItemRepository)
    {
        _logger = logger;
        _sysDirectoryItemRepository = sysDirectoryItemRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist")]
    [Permission("查看", "show")]
    public async Task<SqlSugarPagedList<SysDirectoryItemFullInfo>> GetPageList(SysDirectoryItemPageInput req)
    {
        var data = await _sysDirectoryItemRepository.Change<SysDirectoryItemFullInfo>().ToPageListAsync(req);
        return data;
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    public async Task<SysDirectoryItemFullInfo> GetInfo(int id)
    {
        var entity = await _sysDirectoryItemRepository.Change<SysDirectoryItemFullInfo>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) entity = new SysDirectoryItemFullInfo();
        return entity;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("add")]
    [Permission("添加", "add")]
    [UnitOfWork]
    public async Task<string> Add(SysDirectoryItem req)
    {
        await _sysDirectoryItemRepository.InsertReturnIdentityAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
        return "添加成功";
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("edit"), Permission("修改", "edit")]
    [UnitOfWork]
    public async Task<string> Update(SysDirectoryItem req)
    {
        await _sysDirectoryItemRepository.UpdateAuditAsync(req, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));

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
        await _sysDirectoryItemRepository.DeleteWithSoftAuditAsync(idList, new LogAction(_logPath, _logHandler, CommonHelper.GetClientType()));
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
        var model = await _sysDirectoryItemRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (model == null) throw Oops.Bah("数据不存在");
        var newStatus = !model.Status;
        var newStatusText = newStatus ? "已启用" : "已禁用";

        await _sysDirectoryItemRepository.UpdateAuditAsync(x => new SysDirectoryItem()
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
}

