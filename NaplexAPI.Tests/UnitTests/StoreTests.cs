using Xunit;
using NaplexAPI.Services;
using NaplexAPI.Models.Entities;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;



namespace UnitTests
{
    public class StoreServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly StoreService _storeService;
        private readonly ITestOutputHelper _output;

        public StoreServiceTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"StoreServiceTestDB_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _storeService = new StoreService(_dbContext);
        }

        [Fact]
        public async Task GetAllStoresAsync_ReturnsAllStores()
        {
            _dbContext.Stores.AddRange(
                new Store { Id = 1, StoreName = "Store A" },
                new Store { Id = 2, StoreName = "Store B" }
            );
            await _dbContext.SaveChangesAsync();

            var result = await _storeService.GetAllStoresAsync();

            _output.WriteLine($"Stores returned: {string.Join(", ", result.Select(r => r.StoreName))}");

            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.StoreName == "Store A");
            Assert.Contains(result, s => s.StoreName == "Store B");
        }

        [Fact]
        public async Task GetStoreByIdAsync_ReturnsCorrectStore()
        {
            var store = new Store { Id = 99, StoreName = "Test Store" };
            _dbContext.Stores.Add(store);
            await _dbContext.SaveChangesAsync();

            var result = await _storeService.GetStoreByIdAsync(99);

            _output.WriteLine($"Store returned: {result?.StoreName}");

            Assert.NotNull(result);
            Assert.Equal("Test Store", result.StoreName);
        }

        [Fact]
        public async Task GetStoreByIdAsync_ReturnsNullIfNotFound()
        {
            var result = await _storeService.GetStoreByIdAsync(999);
            _output.WriteLine($"Result for nonexistent store: {(result == null ? "null ✅" : "exists ❌")}");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetStoresForUserAsync_ReturnsStoresLinkedToUser()
        {
            var store = new Store { Id = 1, StoreName = "Main Store" };
            var user = new User { Id = "user123", FirstName = "Jimmy" };

            _dbContext.Stores.Add(store);
            _dbContext.Users.Add(user);
            _dbContext.EmployeeStores.Add(new EmployeeStore
            {
                StoreId = store.Id,
                UserId = user.Id,
                Store = store,
                User = user
            });

            await _dbContext.SaveChangesAsync();

            var result = (await _storeService.GetStoresForUserAsync("user123")).ToList();

            _output.WriteLine($"Stores linked to user: {string.Join(", ", result.Select(r => r.StoreName))}");

            Assert.Single(result);
            Assert.Equal("Main Store", result.First().StoreName);
        }
    }
}
