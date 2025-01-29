using System.ComponentModel.DataAnnotations;

namespace NaplexAPI.Models.DTOs
{
    public class UpdateUserDTO
    {
        [MaxLength(50)]
        public string? FirstName { get; set; }
        [MaxLength(50)]
        public string? LastName { get; set; }
        [MaxLength(100)]
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
    }
}
