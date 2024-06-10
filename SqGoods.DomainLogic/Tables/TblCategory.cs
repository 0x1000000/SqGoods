using SqExpress;
using SqExpress.Syntax.Type;

namespace SqGoods.DomainLogic.Tables
{
    public class TblCategory : TableBase
    {
        public TblCategory(): this(alias: SqExpress.Alias.Auto)
        {
        }

        public TblCategory(Alias alias): base(schema: "dbo", name: "Category", alias: alias)
        {
            this.CategoryId = this.CreateGuidColumn("CategoryId", ColumnMeta.PrimaryKey());
            this.Name = this.CreateStringColumn(name: "Name", size: 255, isUnicode: true, isText: false, columnMeta: null);
            this.Order = this.CreateInt32Column("Order", null);
            this.TopOrder = this.CreateNullableInt32Column("TopOrder", null);
            this.DateTimeCreated = this.CreateDateTimeColumn("DateTimeCreated", false, ColumnMeta.DefaultValue(SqQueryBuilder.GetUtcDate()));
            this.DateTimeUpdated = this.CreateDateTimeColumn("DateTimeUpdated", false, ColumnMeta.DefaultValue(SqQueryBuilder.GetUtcDate()));
            this.AddIndex(this.TopOrder);
            this.AddIndex(this.Order);
            this.AddIndex(this.DateTimeUpdated);
        }

        [SqModel("SgCategoryName", PropertyName = "Id")]
        [SqModel("SgCategory", PropertyName = "Id")]
        public GuidTableColumn CategoryId { get; }

        [SqModel("SgCategoryName")]
        [SqModel("SgCategory")]
        public StringTableColumn Name { get; }

        [SqModel("SgCategory")]
        public Int32TableColumn Order { get; }

        [SqModel("SgCategory")]
        public NullableInt32TableColumn TopOrder { get; }

        public DateTimeTableColumn DateTimeCreated { get; }

        public DateTimeTableColumn DateTimeUpdated { get; }
    }
}