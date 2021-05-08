using System;
using System.Collections.Generic;

namespace SqGoods.Models
{
    public class AttributeSwapFormModel
    {
        public Guid CategoryId { get; set; }

        public IReadOnlyList<AttributeSwapFormItemModel> Swaps { get; set; } =
            Array.Empty<AttributeSwapFormItemModel>();
    }

    public class AttributeSwapFormItemModel
    {
        public Guid OriginalId { get; set; }
        public Guid CurrentId { get; set; }
    }
}