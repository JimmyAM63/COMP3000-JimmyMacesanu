using System.ComponentModel.DataAnnotations;

namespace NaplexAPI.Models.DTOs
{
    public class Register
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StartsWithUpperCase]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        [StartsWithUpperCase]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        [RegularExpression(@"^\S+@\S+\.\S+$", ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone] 
        public string? PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "Store is required.")]
        public int StoreId { get; set; }
    }
}
