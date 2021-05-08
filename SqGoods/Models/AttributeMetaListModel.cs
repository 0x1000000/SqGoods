using System.Collections.Generic;

namespace SqGoods.Models
{
    public class AttributeMetaListModel : DataPageModel<AttributeListModel>
    {
        public AttributeMetaListModel(IReadOnlyList<AttributeListModel> items, int offset, int total, IReadOnlyList<CategoryListModel>? categoryFilter) : base(items, offset, total)
        {
            this.CategoryFilter = categoryFilter;
        }

        public IReadOnlyList<CategoryListModel>? CategoryFilter { get; }
    }
}