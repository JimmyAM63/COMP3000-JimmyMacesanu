using NaplexAPI.Models.DTOs;

namespace NaplexAPI.Services
{
    public interface IChangeRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRolesAsync();
        Task<bool> ChangeUserRoleAsync(ChangeRoleDTO changeRoleDTO);
    }

}
