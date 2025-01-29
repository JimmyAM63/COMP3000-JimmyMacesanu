using Microsoft.AspNetCore.Identity;

namespace NaplexAPI.Models.Entities
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RefreshToken { get; set; }
        public string? Address {get; set; }

        public ICollection<EmployeeStore>? EmployeeStores { get; set; }
    }
}
