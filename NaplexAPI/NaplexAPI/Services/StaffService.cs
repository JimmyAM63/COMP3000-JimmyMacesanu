using NaplexAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaplexAPI.Models.Entities;
using NaplexAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace NaplexAPI.Services
{
    public class StaffService : IStaffService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public StaffService(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserDTO>> GetAllStaffAsync()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = new List<UserDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = roles.FirstOrDefault() // just take the first role
                                                  // ... other properties you want to include
                };
                userDtos.Add(userDto);
            }
            return userDtos;
        }

        public async Task<UserDTO> GetStaffByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = roles.FirstOrDefault() // just take the first role
                                              // ... other properties you want to include
            };
        }

        public async Task<UserDTO> DeleteStaffByIdAsync(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                throw new ApplicationException("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                FirstName= user.FirstName,
                LastName= user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
        }

        public async Task<UserDTO> UpdateYourselfByIdAsync(string id, UpdateUserDTO updatedDetails)
        {
            var user = await _userManager.FindByIdAsync(id); // Find the user with UserManager

            if (user == null)
                throw new ApplicationException("User not found");

            // Update the user details
            user.FirstName = updatedDetails.FirstName;
            user.LastName = updatedDetails.LastName;

            // Check if the email is updated, and use UserManager to update it
            if (updatedDetails.Email != null && updatedDetails.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, updatedDetails.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException("Could not update email.");
                    // You can also handle the errors here based on setEmailResult.Errors
                }
            }

            user.PhoneNumber = updatedDetails.PhoneNumber;
            user.Address = updatedDetails.Address;

            // Update the user with UserManager
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new ApplicationException("Could not update user details.");
                // You can also handle the errors here based on updateResult.Errors
            }

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
        }


        public async Task<IEnumerable<UserDTO>> GetUsersForStoreAsync(int storeId)
        {
            // Step 1: Retrieve users associated with the store.
            var usersForStore = await _context.EmployeeStores
                .Where(es => es.StoreId == storeId)
                .Select(es => es.User)
                .ToListAsync();

            var userDtos = new List<UserDTO>();

            // Step 2: Loop through each user and fetch their role using the UserManager.
            foreach (var user in usersForStore)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = roles.FirstOrDefault() // Assuming each user only has one role
                                                  // ... other properties you want to include
                };

                userDtos.Add(userDto);
            }

            return userDtos;
        }
    }
}
