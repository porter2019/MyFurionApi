namespace MyFurionApi.Application.Controller;

/// <summary>
/// 合同接口
/// </summary>
[PermissionHandler("演示", "合同", "Contract", 10)]
public class ContractController : BaseApiController
{
    private readonly ILogger<ContractController> _logger;
    private readonly SqlSugarRepository<Contract> _contractRepository;

    public ContractController(ILogger<ContractController> logger, SqlSugarRepository<Contract> contractRepository)
    {
        _logger = logger;
        _contractRepository = contractRepository;
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist")]
    public Task<SqlSugarPagedList<Contract>> GetPageList(ContractPageInput req)
    {
        //查询普通列表
        //_contractRepository.ToList(new ContractListInput { Top = 10, Name = "aaa" });

        UnifyContext.Fill(new
        {
            Enums = new List<dynamic> {
                new { Name = "CType", Options = typeof(ContractTypeEnum).GetEnumOptions() },
                new { Name = "Flag", Options = typeof(ContractFlagEnum).GetEnumOptions() }
            }
        });

        return _contractRepository.ToPageListAsync(req);
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    [Permission("查看", "show")]
    public async Task<Contract> GetInfo(int id)
    {
        UnifyContext.Fill(new
        {
            Enums = new List<dynamic> {
                new { Name = "CType", Options = typeof(ContractTypeEnum).GetEnumOptions() },
                new { Name = "Flag", Options = typeof(ContractFlagEnum).GetEnumOptions() }
            }
        });

        //根据Form实体条件自动构建where条件查询
        //var model = _contractRepository.FirstOrDefault(new ContractSingleInput { Code = "aaa" });
        var entity = await _contractRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity != null)
        {
            entity.ItemList = await _contractRepository.Change<ContractItem>().ToListAsync(x => x.ContractId == id);
        }
        else
        {
            entity = new Contract();
        }

        return entity;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("add")]
    [Permission("添加", "add")]
    [UnitOfWork]
    public async Task<string> Add(Contract req)
    {
        req.Code = DateTime.Now.ToFileTime().ToString();
        //添加主体，返回自增id
        var id = await _contractRepository.InsertReturnIdentityAuditAsync(req, new LogAction()
        {
            Local = "演示-合同",
            ExtraHandler = "合同",
            ClientType = CommonHelper.GetClientType(),
        });
        //添加明细，同edit
        await _contractRepository.Change<ContractItem>().UpdateItemDiffAsync(id, req.ItemList);

        return "添加成功";
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("edit")]
    [Permission("修改", "edit")]
    [UnitOfWork]
    public async Task<string> Update(Contract req)
    {

        //更新主体
        await _contractRepository.UpdateAuditAsync(req, new LogAction()
        {
            Local = "演示-合同",
            ExtraHandler = "合同",
            ClientType = CommonHelper.GetClientType(),
        });
        //更新明细，同add
        await _contractRepository.Change<ContractItem>().UpdateItemDiffAsync(req.Id, req.ItemList);

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
        await _contractRepository.Change<ContractItem>().DeleteWithSoftAsync(x => idList.Contains(x.ContractId));
        var total = await _contractRepository.DeleteWithSoftAuditAsync(idList, new LogAction()
        {
            Local = "演示-合同",
            ExtraHandler = "合同",
            ClientType = CommonHelper.GetClientType(),
        });

        return "删除成功";
    }

    /// <summary>
    /// 修改业务状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newFlag"></param>
    /// <returns></returns>
    [HttpGet, Route("modify/flag")]
    public async Task<string> ModifyFlag(int id, int newFlag)
    {
        ContractFlagEnum flag = newFlag.ToEnum<ContractFlagEnum>();
        await _contractRepository.UpdateAuditAsync(x => new Contract()
        {
            Flag = flag,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = "演示-合同",
            ExtraHandler = "合同",
            ExtraInfo = $"新的状态：{flag.GetEnumDescription()}",
            ClientType = CommonHelper.GetClientType(),
        });
        var model = await _contractRepository.FirstOrDefaultAsync(x => x.Id == id);
        return model?.FlagName ?? "";
    }

    /// <summary>
    /// 修改状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("switch/status")]
    public async Task<string> ChangeStatus(int id)
    {
        var entity = await _contractRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw Oops.Bah("数据不存在");
        var newStatus = !entity.Status;
        var newStatusText = newStatus ? "已启用" : "已禁用";
        await _contractRepository.UpdateAuditAsync(x => new Contract()
        {
            Status = newStatus,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = "演示-合同",
            ExtraHandler = "合同",
            ExtraInfo = $"更新后的状态：{newStatusText}",
            ClientType = CommonHelper.GetClientType(),
        });

        return newStatusText;
    }

    #region 导出

    /// <summary>
    /// 导出运输任务
    /// </summary>
    /// <param name="exportService"></param>
    /// <param name="format">格式</param>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, Route("export")]
    public async Task<string> Export([FromServices] IExportService exportService, string format, ContractPageInput req)
    {
        req.PageInfo.PageIndex = 0;
        req.PageInfo.PageSize = 1000;

        var data = await _contractRepository.ToPageListAsync(req);
        if (data.Items.Count() == 0) throw Oops.Bah("数据为空，无法导出");
        var tempData = data.Items;
        foreach (var item in tempData)
        {
            //地址完整拼接一下
            item.RegionFull += item.RegionAddress;
        }

        var exportData = new ExcelExportStandardOutput<Contract>() { DataList = tempData.ToList() };

        return await exportService.Export("导出模板.xlsx", $"Demo-{DateTime.Now.ToFileTime()}", format, exportData);
    }

    #endregion

}