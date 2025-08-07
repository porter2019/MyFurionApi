namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统用户信息
/// </summary>
[FsTable(), SugarIndex("unique_sys_user_cellphone", nameof(CellPhone), OrderByType.Asc, true)]
public class SysUser : BaseEntityStandard
{

    /// <summary>
    /// 手机号
    /// </summary>
    [FsColumn("手机号", 100)]
    public string CellPhone { get; set; }

    /// <summary>
    /// 登录名
    /// </summary>
    [FsColumn("登录名", 100)]
    public string LoginName { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [FsColumn("用户名", 200)]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [FsColumn("密码", 500)]
    public string Password { get; set; }

    /// <summary>
    /// 是否超管
    /// </summary>
    [FsColumn("是否超管", false)]
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 头像
    /// </summary>
    [FsColumn("头像", 500)]
    public string Avatar { get; set; }

    /// <summary>
    /// 头像web地址
    /// </summary>
    [FsColumn(true)]
    public string AvatarPath
    {
        get
        {
            if (Avatar.IsNull()) return "";
            else
            {
                if (Avatar.StartsWith("http")) return Avatar;
                else return App.Configuration[AppSettingsConst.DomainUrl] + Avatar;
            }
        }
    }

    /// <summary>
    /// 账号启用状态
    /// </summary>
    [FsColumn(false)]
    public bool Status { get; set; } = true;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [FsColumn("最后登录时间")]
    public DateTime? LastLoginTime { get; set; }

}

/// <summary>
/// 系统用户运营管理所需的额外数据
/// </summary>
[FsTable("SysUserOMInfoView", true)]
public class SysUserOMInfo : SysUser
{
    /// <summary>
    /// 用户所属角色组Ids
    /// </summary>
    public string RoleIds { get; set; }

    /// <summary>
    /// 用户所属角色组Id
    /// <code>Vue需要数组类型的</code>
    /// </summary>
    [FsColumn(true)]
    public int[] RoleIdArray
    {
        get
        {
            if (RoleIds.IsNull()) return Array.Empty<int>();
            return RoleIds.Split(',').ConvertIntArray();
        }
    }

    /// <summary>
    /// 所属组，逗号分隔
    /// </summary>
    public string RoleNames { get; set; }

}