using System.ComponentModel.DataAnnotations;

namespace Core.Models.Categories
{
    public class CategoryCreateViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
