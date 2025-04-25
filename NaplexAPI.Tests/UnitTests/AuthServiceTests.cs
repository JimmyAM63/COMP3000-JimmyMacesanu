using Xunit;
using Xunit.Abstractions;
using Moq;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using NaplexAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Infrastructure;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace UnitTests
{
    public class UserServiceTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserService _userService;

        public UserServiceTests(ITestOutputHelper output)
        {
            _output = output;

            // UserManager setup
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

            // SignInManager setup
            var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null
            );

            // In-memory DB
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            // Seed a test role and store for registration
            if (!_dbContext.Roles.Any(r => r.Name == "Sales Advisor"))
            {
                _dbContext.Roles.Add(new IdentityRole { Name = "Sales Advisor", NormalizedName = "SALES ADVISOR" });
                if (!_dbContext.Stores.Any(s => s.StoreName == "Test Store"))
                {
                    _dbContext.Stores.Add(new Store { Id = 1, StoreName = "Test Store" });
                }
                _dbContext.SaveChanges();
            }

            // Create service instance
            _userService = new UserService(_userManagerMock.Object, _signInManagerMock.Object, _dbContext);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsUserAndToken()
        {
            // Arrange
            var user = new User { UserName = "testuser", Email = "test@test.com", Id = "123" };
            var login = new Login { UserName = user.UserName, Password = "Password123" };

            _userManagerMock.Setup(m => m.FindByNameAsync(login.UserName)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, login.Password, false))
                              .ReturnsAsync(SignInResult.Success);

            // Act
            var (resultUser, token) = await _userService.LoginAsync(login);

            _output.WriteLine($"🔐 LOGIN TEST (Success)");
            _output.WriteLine($"UserName: {login.UserName}");
            _output.WriteLine($"Password: {login.Password}");
            _output.WriteLine($"Returned Token: {token}");
            _output.WriteLine($"User ID: {resultUser?.Id}, Email: {resultUser?.Email}");

            // Assert
            Assert.NotNull(resultUser);
            Assert.NotNull(token);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ReturnsNull()
        {
            var user = new User { UserName = "testuser", Email = "test@test.com" };
            var login = new Login { UserName = user.UserName, Password = "WrongPassword" };

            _userManagerMock.Setup(m => m.FindByNameAsync(login.UserName)).ReturnsAsync(user);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, login.Password, false))
                              .ReturnsAsync(SignInResult.Failed);

            var (resultUser, token) = await _userService.LoginAsync(login);

            _output.WriteLine($"🔐 LOGIN TEST (Invalid Password)");
            _output.WriteLine($"Attempted User: {login.UserName}");
            _output.WriteLine($"Password Used: {login.Password}");
            _output.WriteLine($"Expected: user = null, token = null");
            _output.WriteLine($"Actual: user = {(resultUser?.UserName ?? "null")}, token = {(token ?? "null")}");

            Assert.Null(resultUser);
            Assert.Null(token);
        }

        [Fact]
        public async Task RegisterAsync_WithValidDetails_CreatesUser()
        {
            var register = new Register
            {
                UserName = "newuser",
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                Password = "Test123!",
                Role = "Sales Advisor",
                StoreId = 1
            };

            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<User>(), register.Password))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<User>(), register.Role))
                            .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<User>()))
                            .ReturnsAsync(new List<string> { register.Role });

            var result = await _userService.RegisterAsync(register);

            _output.WriteLine($"👤 REGISTER TEST");
            _output.WriteLine($"User Email: {register.Email}");
            _output.WriteLine($"Password: {register.Password}");
            _output.WriteLine($"Role: {register.Role}, StoreId: {register.StoreId}");
            _output.WriteLine($"Registered: {result.FirstName} {result.LastName} | {result.Email}");

            Assert.Equal(register.Email, result.Email);
            Assert.Equal(register.FirstName, result.FirstName);
            Assert.Equal(register.Role, result.Role);
        }
    }
}