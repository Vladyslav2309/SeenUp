using System.ComponentModel.DataAnnotations;

namespace Core.Models.Account
{
    public class EditUserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Image { get; set; }
    }
}
