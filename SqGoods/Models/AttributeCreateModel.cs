using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SqGoods.DomainLogic.Models;

namespace SqGoods.Models
{
    public class AttributeCreateModel
    {
        [StringLength(255, MinimumLength = 1)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(SgAttributeType))]
        public SgAttributeType Type { get; set; }

        [Required]
        [MinLength(1)]
        public IReadOnlyList<Guid>? Categories { get; set; } = null;

        [StringLength(255, MinimumLength = 1)]
        public string? Unit { get; set; }

        [Required]
        public bool Mandatory { get; set; }
    }
}