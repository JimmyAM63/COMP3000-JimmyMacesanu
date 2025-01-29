using NaplexAPI.Models.DTOs;

namespace NaplexAPI.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<UserDTO>> GetAllStaffAsync();
        Task<UserDTO> GetStaffByIdAsync(string id);
        Task<IEnumerable<UserDTO>> GetUsersForStoreAsync(int storeId);
        Task<UserDTO> DeleteStaffByIdAsync(string id);
        Task<UserDTO> UpdateYourselfByIdAsync(string id, UpdateUserDTO updatedDetails);
    }
}
