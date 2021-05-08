using System;
using System.ComponentModel.DataAnnotations;

namespace SqGoods.Models
{
    public class AttributeUpdateModel : AttributeCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}