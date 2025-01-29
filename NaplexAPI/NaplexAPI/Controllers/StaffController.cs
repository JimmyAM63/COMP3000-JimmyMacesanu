using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaplexAPI.Services;
using NaplexAPI.Models.DTOs;
using System.Data;
using System.Security.Claims;

namespace NaplexAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IAssignToStoreService _assignService;
        private readonly IChangeRoleService _changeRoleService;

        public StaffController(IStaffService staffService, IAssignToStoreService assignService, IChangeRoleService changeRoleService)
        {
            _staffService = staffService;
            _assignService = assignService;
            _changeRoleService = changeRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(staff);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStaffById(string id)
        {
            //var authenticatedUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            //if (authenticatedUserId == null)
            //{
            //   return Unauthorized("You are not authorized to view details of another user.");
            //}

            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null) return NotFound();
            return Ok(staff);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaffByID(string id)
        {
            try
            {
                var deletedUser = await _staffService.DeleteStaffByIdAsync(id);
                return Ok(deletedUser);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateYourselfById(string id, [FromBody]UpdateUserDTO updatedDetails)
        {
            var authenticatedUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (authenticatedUserId != id)
            {
                return Unauthorized("You are not authorized to edit details of another user.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedUser = await _staffService.UpdateYourselfByIdAsync(id, updatedDetails);
                return Ok(updatedUser);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("stores/{storeId}/users")]
        public async Task<IActionResult> GetUsersByStoreId(int storeId)
        {
            var users = await _staffService.GetUsersForStoreAsync(storeId);

            if (users == null || !users.Any())
            {
                return NotFound($"No users found for store with ID: {storeId}");
            }

            return Ok(users);
        }

        [HttpPost("users/{userId}/assign-store/{storeId}")]
        public async Task<IActionResult> AssignStoreToUser(string userId, int storeId)
        {
            try
            {
                await _assignService.AssignAdditionalStoreToUserAsync(userId, storeId);
                return Ok("Store assigned successfully.");
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _changeRoleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpPut("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromBody] ChangeRoleDTO changeRoleDto)
        {
            var result = await _changeRoleService.ChangeUserRoleAsync(changeRoleDto);
            if (result)
            {
                return Ok("Role changed successfully.");
            }
            return BadRequest("Failed to change role.");
        }

    }
}
