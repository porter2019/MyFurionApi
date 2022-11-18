
namespace MyFurionApi.Application.Entity
{
    /// <summary>
    /// 测试主表
    /// </summary>
    [SugarTable(null)]
    public class Product : BaseEntityStandard
    {
        /// <summary>
        /// 名称
        /// </summary>
        [FsColumn("名称", true, 255)]
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [FsColumn("类型")]
        public ProductType Type { get; set; } = ProductType.Default;

        /// <summary>
        /// 数量
        /// </summary>
        [FsColumn("数量")]
        public int Num { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [FsColumn("金额", ColumnDataType = "money")]
        public decimal Value { get; set; }

        /// <summary>
        /// 明细
        /// </summary>
        [FsColumn("明细", IsIgnore = true)]
        public List<ProductItem> ItemList { get; set; }

    }

    /// <summary>
    /// 类型
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// 默认
        /// </summary>
        [Description("默认")]
        Default = 1,

        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        Custom
    }

}
