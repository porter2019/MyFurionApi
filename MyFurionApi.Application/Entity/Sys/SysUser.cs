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

    }
}
