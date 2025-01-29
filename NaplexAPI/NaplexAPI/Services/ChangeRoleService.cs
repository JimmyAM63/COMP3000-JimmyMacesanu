using Microsoft.AspNetCore.Identity;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace NaplexAPI.Services
{
    public class ChangeRoleService : IChangeRoleService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ChangeRoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRolesAsync()
        {
            return await _roleManager.Roles
                        .Select(r => new RoleDTO { Id = r.Id, Name = r.Name }) // Projecting into the new DTO
                        .ToListAsync();
        }

        public async Task<bool> ChangeUserRoleAsync(ChangeRoleDTO changeRoleDto)
        {
            var user = await _userManager.FindByIdAsync(changeRoleDto.UserId);
            if (user == null) return false;

            // Given you are using RoleId in DTO, you'll need to fetch the role name.
            var role = await _roleManager.FindByIdAsync(changeRoleDto.NewRoleId);
            if (role == null) return false;

            // Clear all current roles for the user
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add the user to the new role
            var result = await _userManager.AddToRoleAsync(user, role.Name);
            return result.Succeeded;
        }
    }

}
