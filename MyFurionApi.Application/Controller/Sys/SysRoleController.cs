namespace MyFurionApi.Application.Controller;

/// <summary>
/// 系统用户组
/// </summary>
[PermissionHandler("系统设置", "角色管理", "SysRole", 0)]
public class SysRoleController : BaseApiController
{
    private readonly ILogger<SysRoleController> _logger;
    private readonly ISysRoleService _sysRoleService;
    private readonly SqlSugarRepository<SysRole> _sysRoleRepository;

    public SysRoleController(ILogger<SysRoleController> logger, ISysRoleService sysRoleService, SqlSugarRepository<SysRole> sysRoleRepository)
    {
        _logger = logger;
        _sysRoleService = sysRoleService;
        _sysRoleRepository = sysRoleRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist")]
    [Permission("查看", "show")]
    public async Task<SqlSugarPagedList<SysRole>> GetPageList(SysRolePageInput req)
    {
        var data = await _sysRoleRepository.ToPageListAsync(req);
        return data;
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    public async Task<SysRole> GetInfo(int id)
    {
        var entity = await _sysRoleRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) entity = new SysRole();
        return entity;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("add")]
    [UnitOfWork]
    [Permission("添加", "add")]
    public async Task<string> Add(SysRole entity)
    {
        entity.IsSuper = false;
        await _sysRoleRepository.InsertReturnIdentityAuditAsync(entity, new LogAction()
        {
            Local = "系统设置-角色管理",
            ExtraHandler = "角色",
            ClientType = CommonHelper.GetClientType(),
        });

        return "添加成功";
    }


    /// <summary>
    /// 检查组名是否存在
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("check/name")]
    public async Task<bool> CheckNameExists(int id, string name)
    {
        return await _sysRoleRepository.Entities.WhereIF(id > 0, x => x.Id != id).AnyAsync(x => x.Name.Equals(name));
    }

    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    [HttpPost, Route("edit"), UnitOfWork]
    [Permission("修改", "edit")]
    public async Task<string> Update(SysRole entity)
    {
        await _sysRoleRepository.UpdateAuditAsync(entity, new LogAction()
        {
            Local = "系统设置-角色管理",
            ExtraHandler = "角色",
            ClientType = CommonHelper.GetClientType(),
        });

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
        await _sysRoleRepository.Change<SysRoleUser>().DeleteAsync(x => idList.Contains(x.RoleId));
        await _sysRoleRepository.DeleteWithSoftAuditAsync(idList, new LogAction()
        {
            Local = "系统设置-角色管理",
            ExtraHandler = "角色",
            ClientType = CommonHelper.GetClientType(),
        });
        return "删除成功";
    }


    /// <summary>
    /// 获取用户组的权限配置列表
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    [HttpGet, Route("get/permit/list")]
    public List<SysRoleModuleGroupOutput> GetRolePermitList(int roleId)
    {
        return _sysRoleService.GetPermitListByRoleId(roleId);
    }

    /// <summary>
    /// 更新用户组的权限信息
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [HttpPost, Route("update/permit")]
    public Task<bool> GetRolePermitList(SysRoleUpdatePermitInput req)
    {
        return _sysRoleService.SetRolePermitAsync(req.RoleId, req.Permits);
    }

    /// <summary>
    /// 根据用户id获取所属的用户组
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet, Route("get/role/list")]
    public Task<List<SysRole>> GetRoleListByUserIdAsync(int userId)
    {
        return _sysRoleService.GetRoleListByUserIdAsync(userId);
    }

    /// <summary>
    /// 根据组ids获取所拥有的权限
    /// </summary>
    /// <param name="roleIds"></param>
    /// <returns></returns>
    [HttpGet, Route("get/role/permits")]
    public Task<List<string>> GetPermissionsByRoleIdAsync(string roleIds)
    {
        return _sysRoleService.GetPermissionsByRoleIdAsync(roleIds);
    }

}
