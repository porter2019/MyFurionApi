namespace MyFurionApi.Application.Entity;

/// <summary>
/// 系统用户信息
/// </summary>
[FsTable()]
public class SysUser : BaseEntityStandard
{
    //, SugarIndex("unique_sys_user_cellphone", nameof(CellPhone), OrderByType.Asc, true)

    /// <summary>
    /// 手机号
    /// </summary>
    [FsColumn(50)]
    public string CellPhone { get; set; }

    /// <summary>
    /// 登录名
    /// </summary>
    [FsColumn(100)]
    public string LoginName { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [FsColumn(200)]
    public string UserName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [FsColumn(500)]
    public string Password { get; set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    [FsColumn]
    public int OrgId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    [FsColumn(30)]
    public string OrgName { get; set; } // 冗余存储

    /// <summary>
    /// 运营系统管理权限
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool IsOM { get; set; }

    /// <summary>
    /// 移动端登录权限
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool IsMP { get; set; }

    /// <summary>
    /// 是否超管
    /// </summary>
    [FsColumn(IsNullable = false)]
    public bool IsSuper { get; set; } = false;

    /// <summary>
    /// 头像
    /// </summary>
    [FsColumn(500)]
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
    /// 最后登录时间
    /// </summary>
    [FsColumn()]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    [FsColumn(30)]
    public string LastLoginIp { get; set; }

}

/// <summary>
/// 系统用户运营管理所需的额外数据
/// </summary>
[FsTable("SysUserOMInfoView", true)]
public class SysUserOMInfo : SysUser
{
    #region 用户组

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

    #endregion

    #region 岗位

    /// <summary>
    /// 用户所属岗位组Ids
    /// </summary>
    public string PostIds { get; set; }

    /// <summary>
    /// 用户所属岗位组Id
    /// <code>Vue需要数组类型的</code>
    /// </summary>
    [FsColumn(true)]
    public int[] PostIdArray
    {
        get
        {
            if (PostIds.IsNull()) return Array.Empty<int>();
            return PostIds.Split(',').ConvertIntArray();
        }
    }

    /// <summary>
    /// 所属组，逗号分隔
    /// </summary>
    public string PostNames { get; set; }

    #endregion

}