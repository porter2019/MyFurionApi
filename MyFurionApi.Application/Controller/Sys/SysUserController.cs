using Furion.DataEncryption.Extensions;
namespace MyFurionApi.Application.Controller;

/// <summary>
/// 系统用户
/// </summary>
[PermissionHandler("系统设置", "用户管理", "SysUser", 0)]
public class SysUserController : BaseApiController
{
    private readonly ILogger<SysUserController> _logger;
    private readonly SqlSugarRepository<SysUser> _sysUserRepository;
    private static readonly string _displayPassword = "SS@@****@@SS";

    public SysUserController(ILogger<SysUserController> logger,
        SqlSugarRepository<SysUser> sysUserRepository)
    {
        _logger = logger;
        _sysUserRepository = sysUserRepository;
    }

    /// <summary>
    /// 获取列表
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("get/pagelist")]
    [Permission("查看", "show")]
    public async Task<SqlSugarPagedList<SysUserOMInfo>> GetPageList(SysUserPageInput req)
    {
        var data = await _sysUserRepository.Change<SysUserOMInfo>().ToPageListAsync(req);
        foreach (var item in data.Items)
        {
            item.Password = null;
            //查询当前的角色信息
            //var roleList = await _sysUserRepository.Context.Queryable<SysRole>().RightJoin<SysRoleUser>((a, b) => a.Id == b.RoleId && b.UserId == item.Id).ToListAsync();
            //item.RoleIds = roleList.Select(x => x.Id).ToList().Join();
            //item.RoleNames = roleList.Select(x => x.Name).ToList().Join();
        }
        return data;
    }

    /// <summary>
    /// 获取详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("get/info")]
    public async Task<SysUserOMInfo> GetInfo(int id)
    {
        var entity = await _sysUserRepository.Change<SysUserOMInfo>().FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) entity = new SysUserOMInfo() { IsOM = true, IsSuper = false };
        else entity.Password = _displayPassword;
        return entity;
    }

    /// <summary>
    /// 检查手机号是否存在
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("check/cellphone")]
    public async Task<bool> CheckCellPhoneExists(int id, string cellPhone)
    {
        return await _sysUserRepository.Entities.WhereIF(id > 0, x => x.Id != id).AnyAsync(x => x.CellPhone.Equals(cellPhone));
    }

    /// <summary>
    /// 重置账户密码
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("reset/password")]
    public async Task<string> ResetPassword(int id)
    {
        var defaultPassword = "123456";
        var userEntity = await _sysUserRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (userEntity == null) throw Oops.Oh("账号不存在");
        var newPassword = defaultPassword.ToMD5Encrypt();
        await _sysUserRepository.UpdateAsync(x => new SysUser() { Password = newPassword }, x => x.Id == userEntity.Id);
        return defaultPassword;
    }

    /// <summary>
    /// 添加
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("add")]
    [Permission("添加", "add")]
    [UnitOfWork]
    public async Task<string> Add(SysUserOMInfo entity)
    {
        entity.IsSuper = false;
        entity.Password = entity.Password.ToMD5Encrypt();
        var userId = await _sysUserRepository.InsertReturnIdentityAuditAsync(entity, new LogAction()
        {
            Local = "系统设置-用户管理",
            ExtraHandler = "用户",
            ClientType = CommonHelper.GetClientType(),
        });
        var userRoleList = new List<SysRoleUser>();
        foreach (var item in entity.RoleIds.SplitWithComma())
        {
            userRoleList.Add(new SysRoleUser()
            {
                UserId = userId,
                RoleId = Convert.ToInt32(item)
            });
        }
        await _sysUserRepository.Change<SysRoleUser>().InsertAsync(userRoleList);
        return "添加成功";
    }


    /// <summary>
    /// 更新
    /// </summary>
    /// <returns></returns>
    [HttpPost, Route("edit")]
    [Permission("修改", "edit")]
    [UnitOfWork]
    public async Task<string> Update(SysUserOMInfo entity)
    {
        if (!entity.Password.Equals(_displayPassword))
        {
            //修改了密码
            entity.Password = entity.Password.ToMD5Encrypt();
            await _sysUserRepository.UpdateAuditAsync(entity, new LogAction()
            {
                Local = "系统设置-用户管理",
                ExtraHandler = "用户",
                ClientType = CommonHelper.GetClientType(),
            });
        }
        else
        {
            await _sysUserRepository.UpdateAuditAsync(entity, new LogAction()
            {
                Local = "系统设置-用户管理",
                ExtraHandler = "用户",
                ClientType = CommonHelper.GetClientType(),
            }, "Password");
        }

        //修改角色组信息
        await _sysUserRepository.Change<SysRoleUser>().DeleteAsync(x => x.UserId == entity.Id);
        var userRoleList = new List<SysRoleUser>();
        foreach (var item in entity.RoleIds.SplitWithComma())
        {
            userRoleList.Add(new SysRoleUser()
            {
                UserId = entity.Id,
                RoleId = Convert.ToInt32(item)
            });
        }
        await _sysUserRepository.Change<SysRoleUser>().InsertAsync(userRoleList);
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
        await _sysUserRepository.Change<SysRoleUser>().DeleteWithSoftAsync(x => idList.Contains(x.UserId));
        await _sysUserRepository.DeleteWithSoftAuditAsync(idList, new LogAction()
        {
            Local = "系统设置-用户管理",
            ExtraHandler = "用户",
            ClientType = CommonHelper.GetClientType(),
        });
        await _sysUserRepository.EntityContext.Updateable<SysUser>()
                                    .PublicSetColumns(x => x.CellPhone, x => x.CellPhone + "-" + DateTime.Now.ToTimeStamp().ToString()) //将手机号后缀加上-时间戳，防止手机号重复
                                    .Where(x => idList.Contains(x.Id))
                                    .ExecuteCommandAsync();
        return "删除成功";
    }

    /// <summary>
    /// 修改账户状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("switch/status")]
    public async Task<string> ChangeStatus(int id)
    {
        var entity = await _sysUserRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw Oops.Bah("账号不存在");
        var newStatus = !entity.Status;
        var newStatusText = newStatus ? "已启用" : "已禁用";
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            Status = newStatus,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = "系统设置-用户管理",
            ExtraHandler = "用户",
            ExtraInfo = $"更新后的用户状态：{newStatusText}",
            ClientType = CommonHelper.GetClientType(),
        });
        return newStatusText;
    }

    /// <summary>
    /// 修改后台权限状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("switch/isom")]
    public async Task<string> ChangeIsOM(int id)
    {
        var entity = await _sysUserRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw Oops.Bah("账号不存在");
        var newStatus = !entity.IsOM;
        var newStatusText = newStatus ? "已启用" : "已禁用";
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            IsOM = newStatus,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = "系统设置-用户管理",
            ExtraHandler = "用户",
            ExtraInfo = $"更新后的后台登录权限状态：{newStatusText}",
            ClientType = CommonHelper.GetClientType(),
        });
        return newStatusText;
    }

    /// <summary>
    /// 修改移动端状态
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet, Route("switch/ismp")]
    public async Task<string> ChangeIsMP(int id)
    {
        var entity = await _sysUserRepository.FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null) throw Oops.Bah("账号不存在");
        var newStatus = !entity.IsMP;
        var newStatusText = newStatus ? "已启用" : "已禁用";
        await _sysUserRepository.UpdateAuditAsync(x => new SysUser()
        {
            IsMP = newStatus,
            UpdatedTime = DateTime.Now,
            UpdatedUserId = CurrentUserId,
            UpdatedUserName = CurrentUserName
        }, x => x.Id == id, new LogAction()
        {
            Local = "系统设置-用户管理",
            ExtraHandler = "用户",
            ExtraInfo = $"更新后的移动端登录权限状态：{newStatusText}",
            ClientType = CommonHelper.GetClientType(),
        });
        return newStatusText;
    }

    ///// <summary>
    ///// 修改手机号
    ///// </summary>
    ///// <param name="req"></param>
    ///// <returns></returns>
    //[HttpPost, Route("modify/cellphone")]
    //public async Task<string> ChangeCellPhone(SysUserChangeCellPhoneInput req)
    //{
    //    var checkCodeResult = await _smsService.ValidateCodeAsync(req.Guid, req.VCode);
    //    if (!checkCodeResult.IsSuccessed) throw Oops.Bah(checkCodeResult.Message);

    //    if (await CheckCellPhoneExists(req.CurrentUserId, req.CellPhone)) throw Oops.Bah("手机号码已存在");
    //    await _sysUserRepository.UpdateAsync(x => new SysUser() { CellPhone = req.CellPhone, UpdatedUserId = req.CurrentUserId, UpdatedUserName = req.CurrentUserName, UpdatedTime = DateTime.Now }, x => x.Id == CurrentUserId);

    //    return "修改成功";
    //}


}
