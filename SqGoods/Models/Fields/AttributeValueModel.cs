using System;

namespace SqGoods.Models.Fields
{
    public class AttributeValueModel
    {
        public Guid AttributeId { get; set; }

        public string? Value { get; set; }
    }
}