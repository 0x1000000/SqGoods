using System;
using System.Collections.Generic;

namespace SqGoods.Models
{
    public class ProductListModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string CoverUrl { get; set; } = string.Empty;

        public IReadOnlyList<ProductListAttributeModel> Attributes { get; set; } =
            Array.Empty<ProductListAttributeModel>();
    }

    public class ProductListAttributeModel
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;
    }
}