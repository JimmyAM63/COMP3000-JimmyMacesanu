using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using NaplexAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NaplexAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;


        public AuthController(IUserService userService, IConfiguration configuration, UserManager<User> userManager)
        {
            _userService = userService;
            _configuration = configuration;
            _userManager = userManager;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userDto = await _userService.RegisterAsync(register);
                return Ok(userDto);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            (User? user, string refreshToken) = await _userService.LoginAsync(login);

            if (user == null)
            {
                return Unauthorized();
            }

            var roles = await _userManager.GetRolesAsync(user); // Fetch roles
            var claims = new List<Claim>
            {
                new Claim("username", user.UserName), // Username claim
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)  // IMPORTANT: User ID claim
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Add roles as claims
            }

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["TokenExpirationMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var createdToken = tokenHandler.CreateToken(tokenDescriptor);

            Response.Cookies.Append(
                "refreshToken",
                refreshToken,
                new CookieOptions 
                { 
                    HttpOnly = true, 
                    Secure = true,
                    SameSite = SameSiteMode.None 
                }
           );

            return Ok(new { access_token = tokenHandler.WriteToken(createdToken),
                            refresh_token = refreshToken });
        }

        // POST: api/auth/logout
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                Console.WriteLine($"User ID from JWT: {userId}"); // Or use logging if you have it set up
                await RemoveRefreshToken(userId);
                Response.Cookies.Delete("refreshToken");
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<bool> RemoveRefreshToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && !string.IsNullOrEmpty(user.RefreshToken))
            {
                user.RefreshToken = null;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to update user. Errors: {errors}");
                }
                return result.Succeeded;
            }
            throw new Exception("User not found or refresh token is empty");
        }







    }
}
