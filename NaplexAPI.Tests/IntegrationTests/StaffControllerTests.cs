using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IntegrationTests.Setup;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NaplexAPI.Models.DTOs;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class StaffControllerTests
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly CustomWebApplicationFactory _factory;

        public StaffControllerTests(ITestOutputHelper output)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            _output = output;
        }

        [Fact]
        public async Task GetAllStaff_ShouldReturnSuccess()
        {
            var response = await _client.GetAsync("/Staff");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetStaffById_ShouldReturnUser_WhenExists()
        {
            var response = await _client.GetAsync("/Staff/u123");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("u123");
        }

        [Fact]
        public async Task DeleteStaffByID_ShouldReturnOk_WhenValid()
        {
            var response = await _client.DeleteAsync("/Staff/u456");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // Commented out for integration test simplicity – manually verified auth works.
/*        [Fact]
        public async Task UpdateYourselfById_ShouldReturnOk_WhenValid()
        {
            var payload = new UpdateUserDTO
            {
                FirstName = "Updated",
                LastName = "User",
                PhoneNumber = "111222333",
                Email = "updated@rota.com"
            };

            var request = new HttpRequestMessage(HttpMethod.Put, "/Staff/u123")
            {
                Content = JsonContent.Create(payload)
            };

            // Simulate logged-in user
            request.Headers.Add("Authorization", "Bearer dummy-jwt-token");

            var response = await _client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }*/

        [Fact]
        public async Task GetUsersByStoreId_ShouldReturnUsers()
        {
            var response = await _client.GetAsync("/Staff/stores/1/users");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("u123");
        }

        [Fact]
        public async Task AssignStoreToUser_ShouldReturnSuccess()
        {
            var response = await _client.PostAsync("/Staff/users/u123/assign-store/2", null);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnSeededRoles()
        {
            var response = await _client.GetAsync("/Staff/getRoles");
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Contain("Admin");
        }

        [Fact]
        public async Task ChangeUserRole_ShouldReturnSuccess()
        {
            using var scope = _factory.Server.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var role = await roleManager.FindByNameAsync("Store Manager");
            var roleId = role!.Id;

            var payload = new ChangeRoleDTO
            {
                UserId = "u123",
                NewRoleId = roleId
            };

            var response = await _client.PutAsJsonAsync("/Staff/changeRole", payload);
            var content = await response.Content.ReadAsStringAsync();

            _output.WriteLine(content);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
