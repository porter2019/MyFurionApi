namespace MyFurionApi.Application.Entity
{
    /// <summary>
    /// 系统模块下的功能
    /// </summary>
    [SugarTable(tableName: null)]
    public class SysHandler : BaseEntity
    {
        /// <summary>
        /// 所属模块Id
        /// </summary>
        [FsColumn("模块Id")]
        public int ModuleId { get; set; }

        /// <summary>
        /// 功能名称
        /// </summary>
        [FsColumn("功能名称", false, 50)]
        public string HandlerName { get; set; }

        /// <summary>
        /// 功能别名
        /// </summary>
        [FsColumn("功能别名", false, 50)]
        public string AliasName { get; set; }

        /// <summary>
        /// 关联控制器
        /// </summary>
        [FsColumn("关联控制器", false, 500)]
        public string RefController { get; set; }

        /// <summary>
        /// 排序数字
        /// </summary>
        [FsColumn("排序数字")]
        public int OrderNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [FsColumn("状态")]
        public bool Status { get; set; } = true;
    }
}