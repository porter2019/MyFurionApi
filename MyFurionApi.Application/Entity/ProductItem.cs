namespace MyFurionApi.Application.Entity
{
    /// <summary>
    /// 产品明细
    /// </summary>
    [SugarTable(null), SugarIndex("unique_productitem_num", nameof(Num), OrderByType.Asc, true)]
    public class ProductItem : BaseEntity
    {
        /// <summary>
        /// 产品id
        /// </summary>
        [FsColumn("产品id"), ForeignKeyTag]
        public int ProductId { get; set; }

        /// <summary>
        /// 唯一索引约束
        /// </summary>
        [FsColumn("唯一")]
        public int Num { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [FsColumn("金额", ColumnDataType = "money")]
        public decimal Money { get; set; }

        /// <summary>
        /// 字符串
        /// </summary>
        [FsColumn("字符串", true, 200)]
        public string Str { get; set; }

    }
}
