using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;

namespace NaplexAPI.Services
{
    public interface IUserService
    {
        Task<UserDTO> RegisterAsync(Register register);
        Task<(User?, string)> LoginAsync(Login login);
        // Other user-specific operations like UpdateUser, DeleteUser, GetUserById etc.
    }
}
