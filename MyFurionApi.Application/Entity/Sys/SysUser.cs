namespace MyFurionApi.Application.Entity
{
    /// <summary>
    /// 系统用户信息
    /// </summary>
    [SugarTable(null), SugarIndex("unique_sys_user_loginname", nameof(LoginName), OrderByType.Asc, true)]
    public class SysUser : BaseEntityStandard
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [FsColumn("登录名", false, 100)]
        public string LoginName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [FsColumn("用户名", true, 200)]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [FsColumn("密码", false, 500)]
        public string Password { get; set; }

        /// <summary>
        /// 是否超管
        /// </summary>
        [FsColumn("是否超管")]
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// 头像
        /// </summary>
        [FsColumn("头像", true, 500)]
        public string Avatar { get; set; }

        /// <summary>
        /// 头像web地址
        /// </summary>
        [FsColumn("头像web地址", IsIgnore = true)]
        public string AvatarPath 
        {
            get
            {
                return Avatar.IsNull() ? "" : App.Configuration[AppSettingsConst.DomainUrl] + Avatar;
            }
        }

        /// <summary>
        /// 账号启用状态
        /// </summary>
        [FsColumn("账号状态")]
        public bool Status { get; set; } = true;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [FsColumn("最后登录时间")]
        public DateTime? LastLoginTime { get; set; }

    }
}
