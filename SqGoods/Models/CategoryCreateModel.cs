using System.ComponentModel.DataAnnotations;

namespace SqGoods.Models
{
    public class CategoryCreateModel
    {
        [StringLength(255, MinimumLength = 1)]
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue)]
        public int Order { get; set; }

        [Range(0, int.MaxValue)]
        public int? TopOrder { get; set; }
    }
}