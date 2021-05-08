using System;
using System.ComponentModel.DataAnnotations;

namespace SqGoods.Models
{
    public class AttributeItemModel
    {
        public Guid? Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;
    }
}