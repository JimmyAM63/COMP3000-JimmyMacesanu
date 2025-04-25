using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using IntegrationTests.Setup;
using NaplexAPI.Models.DTOs;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.AspNetCore.Identity;
using NaplexAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class AuthControllerTests
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper output;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerTests(ITestOutputHelper output)
        {
            _factory = new CustomWebApplicationFactory("TestDb_Auth");

            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            var clientOptions = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            };

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true
            });

            this.output = output;
        }

        [Fact]
        public async Task Register_ShouldRegisterNewUserSuccessfully()
        {
            var registerDto = new
            {
                UserName = "newuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = "Store Manager", // Should exactly match seeded roles
                StoreId = 1
            };

            var response = await _client.PostAsJsonAsync("/Auth/register", registerDto);
            var contentString = await response.Content.ReadAsStringAsync();
            output.WriteLine($"Response Content: {contentString}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_ShouldLoginUserSuccessfully()
        {
            var registerDto = new
            {
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PhoneNumber = "1234567890",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Role = "Admin",
                StoreId = 1
            };
            await _client.PostAsJsonAsync("/Auth/register", registerDto);

            // ✅ Check if registration really worked
            var scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByNameAsync("testuser");

                output.WriteLine(user == null ? "❌ User not found" : $"✅ User found: {user.UserName}");

                if (user != null)
                {
                    var isPasswordValid = await userManager.CheckPasswordAsync(user, "Password123!");
                    output.WriteLine(isPasswordValid ? "✅ Password valid" : "❌ Invalid password");

                    var roles = await userManager.GetRolesAsync(user);
                    output.WriteLine("Assigned roles: " + string.Join(", ", roles));

                    output.WriteLine("Refresh token: " + (user.RefreshToken ?? "❌ No refresh token"));
                }
            }

            // ✅ Proceed to login
            var loginDto = new { UserName = "testuser", Password = "Password123!" };
            var response = await _client.PostAsJsonAsync("/Auth/login", loginDto);
            var contentString = await response.Content.ReadAsStringAsync();
            output.WriteLine($"Response Content: {contentString}");

            response.StatusCode.Should().Be(HttpStatusCode.OK, $"Login failed: {contentString}");

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            content.TryGetProperty("access_token", out var token).Should().BeTrue("access_token not found");
            token.GetString().Should().NotBeNullOrEmpty("access_token was null or empty");
        }
    }
}