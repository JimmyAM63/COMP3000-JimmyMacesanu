using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using System.Security.Cryptography;

namespace NaplexAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;
        // other dependencies like IConfiguration for JWT, DbContext for direct database operations, etc.

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<UserDTO> RegisterAsync(Register register)
        {
            // Convert DTO to User entity
            var user = new User
            {
                UserName = register.UserName,
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                PhoneNumber = register.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                throw new ApplicationException($"User registration failed: {string.Join(", ", result.Errors.Select(x => x.Description))}");
            }

            // Ensure role exists and add role to user
            var roleExists = await _context.Roles.AnyAsync(r => r.Name == register.Role);
            if (!roleExists)
            {
                throw new ApplicationException($"Role '{register.Role}' does not exist.");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, register.Role);
            if (!roleResult.Succeeded)
            {
                throw new ApplicationException($"Role assignment failed: {string.Join(", ", roleResult.Errors.Select(x => x.Description))}");
            }

            // Check if the store exists in the database
            var storeExists = await _context.Stores.AnyAsync(s => s.Id == register.StoreId);
            if (!storeExists)
            {
                throw new ApplicationException("Store not found.");
            }

            // Assign the primary store to the user
            var employeeStore = new EmployeeStore
            {
                UserId = user.Id,
                StoreId = register.StoreId,
                IsPrimary = true
            };

            _context.EmployeeStores.Add(employeeStore);
            await _context.SaveChangesAsync();

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault()
                // Set other properties if you've added more to the DTO
            };
        }

        public async Task<(User?, string)> LoginAsync(Login login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null)
            {
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
                if (signInResult.Succeeded)
                {
                    var refreshToken = GenerateRefreshToken();
                    await SaveRefreshToken(user.Id, refreshToken);
                    return (user, refreshToken); // Return user object
                }
            }
            return (null, null);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<bool> SaveRefreshToken(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);
                return true;
            }
            return false;
        }


        // Implement other methods as per the interface
    }
}
