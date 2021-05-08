using System;
using System.ComponentModel.DataAnnotations;

namespace SqGoods.Models
{
    public class CategoryUpdateModel : CategoryCreateModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
