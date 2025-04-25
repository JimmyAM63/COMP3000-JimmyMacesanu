using Xunit;
using Moq;
using NaplexAPI.Services;
using NaplexAPI.Models.Entities;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit.Abstractions;
using System.Linq;



namespace UnitTests
{
    public class StaffServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly StaffService _staffService;
        private readonly ITestOutputHelper _output;
        private readonly User _sharedSeededUser;

        private void SeedSharedUsers()
        {
            var sharedUsers = new List<User>
            {
                new User
                {
                    Id = "u123",
                    FirstName = "Jimmy",
                    LastName = "Macesanu",
                    Email = "jimmy@example.com",
                    PhoneNumber = "1234567890",
                    Address = "CF Street 12"
                },
                new User
                {
                    Id = "sg463adhg",
                    FirstName = "Laurentiu",
                    LastName = "Istrate",
                    Email = "flv@example.com",
                    PhoneNumber = "0251261855",
                    Address = "CF Street 13"
                }
            };

            // Add to database only if not already there
            foreach (var user in sharedUsers)
            {
                if (!_dbContext.Users.Any(u => u.Id == user.Id))
                {
                    _dbContext.Users.Add(user);
                }
            }

            _dbContext.SaveChanges();

            // Setup roles for seeded users
            var rolesDict = new Dictionary<string, List<string>>
            {
                { "u123", new List<string> { "Admin" } },
                { "sg463adhg", new List<string> { "Sales Advisor" } }
            };

            _userManagerMock
                .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => rolesDict.ContainsKey(u.Id) ? rolesDict[u.Id] : new List<string>());
        }

        public StaffServiceTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "StaffServiceTestDB")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            //_dbContext.Users.RemoveRange(_dbContext.Users);
            //_dbContext.SaveChanges();

            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            _staffService = new StaffService(_dbContext, _userManagerMock.Object);

            SeedSharedUsers();

            _sharedSeededUser = _dbContext.Users.First(u => u.Id == "u123");
        }

        [Fact]
        public async Task GetAllStaffAsync_ReturnsAllSeededUsersWithRoles()
        {
            // Act
            var result = await _staffService.GetAllStaffAsync();

            // Output
            foreach (var user in result)
            {
                _output.WriteLine($"User: {user.FirstName} {user.LastName} | Email: {user.Email} | Role: {user.Role}");
                _output.WriteLine($"Phone: {user.PhoneNumber} | Address: {user.Address}");
            }

            // Assert
            Assert.Equal(2, result.Count()); // we expect only the seeded users now
            Assert.Contains(result, u => u.Id == "u123" && u.Role == "Admin");
            Assert.Contains(result, u => u.Id == "sg463adhg" && u.Role == "Sales Advisor");
        }

        [Fact]
        public async Task GetStaffByIdAsync_ReturnsCorrectUserDTO()
        {
            // Act
            var result = await _staffService.GetStaffByIdAsync("u123");

            // Output
            _output.WriteLine($"Returned: {result.FirstName} {result.LastName}, Role: {result.Role}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jimmy", result.FirstName);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task DeleteStaffByIdAsync_DeletesSeededUser()
        {
            // Act
            var result = await _staffService.DeleteStaffByIdAsync("sg463adhg");

            var exists = await _dbContext.Users.FindAsync("sg463adhg");

            // Output
            _output.WriteLine($"Delete result: {result}");
            _output.WriteLine($"User still in DB? {(exists == null ? "No ✅" : "Yes ❌")}");
            _output.WriteLine($"Deleted user: {result.FirstName} {result.LastName}");
            _output.WriteLine($"Deleted user's Role: {result.Role}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laurentiu", result.FirstName);
            Assert.Null(exists);
        }

        [Fact]
        public async Task UpdateYourselfByIdAsync_UpdatesSeededUserSuccessfully()
        {

            // Arrange
            var userId = _sharedSeededUser.Id;

            // Clone the shared user to avoid modifying the global seed
            var userToUpdate = new User
            {
                Id = _sharedSeededUser.Id,
                FirstName = _sharedSeededUser.FirstName,
                LastName = _sharedSeededUser.LastName,
                Email = _sharedSeededUser.Email,
                PhoneNumber = _sharedSeededUser.PhoneNumber,
                Address = _sharedSeededUser.Address
            };

            var updatedDetails = new UpdateUserDTO
            {
                FirstName = "Jimbo",
                LastName = "Mace",
                Email = "jimmyupdated@example.com",
                PhoneNumber = "9876543210",
                Address = "Updated Avenue 99"
            };

            _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                .ReturnsAsync(userToUpdate);

            _userManagerMock.Setup(m => m.SetEmailAsync(userToUpdate, updatedDetails.Email))
                .Callback<User, string>((u, newEmail) => u.Email = newEmail) // <-- this line actually updates the email
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(m => m.UpdateAsync(userToUpdate))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _staffService.UpdateYourselfByIdAsync(userId, updatedDetails);

            // Output
            _output.WriteLine("🔁 UPDATE TEST");
            _output.WriteLine($"Before: {_sharedSeededUser.FirstName} {_sharedSeededUser.LastName}");
            _output.WriteLine($"{_sharedSeededUser.Email} {_sharedSeededUser.PhoneNumber}");
            _output.WriteLine($"{_sharedSeededUser.Address}");
            _output.WriteLine($"After: {result.FirstName} {result.LastName}");
            _output.WriteLine($"{result.Email} {result.PhoneNumber}");
            _output.WriteLine($"{result.Address}");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedDetails.FirstName, result.FirstName);
            Assert.Equal(updatedDetails.LastName, result.LastName);
            Assert.Equal(updatedDetails.Email, result.Email);
            Assert.Equal(updatedDetails.PhoneNumber, result.PhoneNumber);
            Assert.Equal(updatedDetails.Address, result.Address);
        }

        [Fact]
        public async Task GetUsersForStoreAsync_ReturnsUsersAssignedToStore()
        {
            // Arrange
            var storeId = 123;

            var store = new Store
            {
                Id = storeId,
                StoreName = "Main Store"
            };

            var user = new User
            {
                Id = "u999",
                FirstName = "Test",
                LastName = "User",
                Email = "testuser@example.com"
            };

            var employeeStore = new EmployeeStore
            {
                StoreId = storeId,
                UserId = user.Id,
                Store = store,
                User = user
            };

            _dbContext.Stores.Add(store);
            _dbContext.Users.Add(user);
            _dbContext.EmployeeStores.Add(employeeStore);
            await _dbContext.SaveChangesAsync();

            _userManagerMock
                .Setup(m => m.GetRolesAsync(It.Is<User>(u => u.Id == user.Id)))
                .ReturnsAsync(new List<string> { "Sales Advisor" });

            // Act
            var result = (await _staffService.GetUsersForStoreAsync(storeId)).ToList();

            // Output
            foreach (var u in result)
            {
                _output.WriteLine($"User: {u.FirstName} {u.LastName} | Role: {u.Role} | Email: {u.Email}");
                _output.WriteLine($"Store Name: {store.StoreName}");
            }

            // Assert
            Assert.Single(result);
            Assert.Equal("Test", result[0].FirstName);
            Assert.Equal("Sales Advisor", result[0].Role);

            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();
        }
    }
}
