using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UserMVC.Models
{
    public class User
    {
        public int Id { get; set; }

        [DisplayName("First Name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Age")]
        [Required]
        [Range(1,120)]
        public int Age { get; set; }

        [DisplayName("Email")]
        [Required]
        //[EmailAddress]
        [RegularExpression(@"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }
    }
}
