using System.ComponentModel.DataAnnotations;

namespace Core.Models.Categories
{
    public class CategoryEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
