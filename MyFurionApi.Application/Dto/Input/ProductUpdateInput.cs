namespace MyFurionApi.Application.Dto
{
    /// <summary>
    /// 更新传入的
    /// </summary>
    public class ProductUpdateInput : BaseFormPostModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public ProductType Type { get; set; } = ProductType.Default;

        /// <summary>
        /// 数量
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Value { get; set; }
        
    }
}
