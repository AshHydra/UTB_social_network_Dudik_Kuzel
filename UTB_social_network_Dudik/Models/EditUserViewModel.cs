using System.ComponentModel.DataAnnotations;

namespace UTB_social_network_Dudik.Models
{
    public class EditUserViewModel
    {
        public int Id { get; set; } // User ID

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Roles (IDs)")]
        public string RoleIds { get; set; } // A comma-separated string of role IDs
    }
}
