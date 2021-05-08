namespace SqGoods.Models
{
    public class CategoryListModel : CategoryUpdateModel
    {
        public int AttributesCount { get; set; }

        public int ProductCount { get; set; }
    }
}