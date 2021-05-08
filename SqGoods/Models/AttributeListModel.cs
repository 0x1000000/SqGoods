using System;
using System.ComponentModel.DataAnnotations;
using SqGoods.DomainLogic.Models;

namespace SqGoods.Models
{
    public class AttributeListModel
    {
        public Guid Id { get; set; }

        public int? OrderInCategory { get; set; }

        public string Name { get; set; } = string.Empty;

        [EnumDataType(typeof(SgAttributeType))]
        public SgAttributeType Type { get; set; }

        public string? Unit { get; set; }

        public bool? Mandatory { get; set; }

        public int NumberOfCategories { get; set; }

        public int? NumberOfItems { get; set; }
    }
}