using System.ComponentModel.DataAnnotations;

namespace NaplexAPI.Models.DTOs
{
    public class Login
    {
        [Required(ErrorMessage = "User Name is required.")]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
