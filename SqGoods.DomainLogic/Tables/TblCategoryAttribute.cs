using SqExpress;

namespace SqGoods.DomainLogic.Tables
{
    public class TblCategoryAttribute : TableBase
    {
        public TblCategoryAttribute(): this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblCategoryAttribute(Alias alias): base(schema: "dbo", name: "CategoryAttribute", alias: alias)
        {
            this.CategoryId = this.CreateGuidColumn("CategoryId", ColumnMeta.PrimaryKey().ForeignKey<TblCategory>(t => t.CategoryId));
            this.AttributeId = this.CreateGuidColumn("AttributeId", ColumnMeta.PrimaryKey().ForeignKey<TblAttribute>(t => t.AttributeId));
            this.Order = this.CreateInt32Column("Order");
            this.Mandatory = this.CreateBooleanColumn("Mandatory", ColumnMeta.DefaultValue(false));
        }

        [SqModel("SgCategoryAttributeOrder")]
        [SqModel("SgCategoryAttributeMandatory")]
        public GuidTableColumn CategoryId { get; }

        [SqModel("SgCategoryAttributeOrder")]
        [SqModel("SgCategoryAttributeMandatory")]
        public GuidTableColumn AttributeId { get; }

        [SqModel("SgCategoryAttributeOrder")]
        public Int32TableColumn Order { get; }

        [SqModel("SgCategoryAttributeMandatory")]
        public BooleanTableColumn Mandatory { get; }
    }
}