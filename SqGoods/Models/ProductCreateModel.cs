using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SqGoods.Models.Fields;

namespace SqGoods.Models
{
    public class ProductCreateModel
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public Guid CategoryId { get; set; } = Guid.Empty;

        public IReadOnlyList<AttributeValueModel> Values { get; set; } = Array.Empty<AttributeValueModel>();
    }
}