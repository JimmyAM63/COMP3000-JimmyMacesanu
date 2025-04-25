using Xunit;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.Entities;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;



namespace UnitTests
{
    public class RotaServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RotaService _rotaService;
        private readonly ITestOutputHelper _output;

        public RotaServiceTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"RotaServiceTestDB_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _rotaService = new RotaService(_dbContext);

            // Seed shared data
            var store = new Store { Id = 2, StoreName = "Main Store" };
            var user = new User { Id = "u123", FirstName = "Jimmy" };

            _dbContext.Stores.Add(store);
            _dbContext.Users.Add(user);
            _dbContext.EmployeeStores.Add(new EmployeeStore
            {
                StoreId = store.Id,
                UserId = user.Id,
                Store = store,
                User = user
            });

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateRotaAsync_CreatesRotaSuccessfully()
        {
            var rotaDto = new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 10),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            };

            var result = await _rotaService.CreateRotaAsync(rotaDto);

            _output.WriteLine($"Created rota for {result.UserId} on {result.Date:yyyy-MM-dd} from {result.StartTime} to {result.EndTime}");
            Assert.NotNull(result);
            Assert.Equal("u123", result.UserId);
        }

        [Fact]
        public async Task GetRotaByIdAsync_ReturnsCorrectRota()
        {
            var created = await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 10),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            });

            var fetched = await _rotaService.GetRotaByIdAsync(created.RotaId);
            _output.WriteLine($"Fetched Rota ID: {fetched.RotaId}, Date: {fetched.Date:yyyy-MM-dd}");
            Assert.NotNull(fetched);
        }

        [Fact]
        public async Task GetRotasForStoreAsync_ReturnsRotaList()
        {
            await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 11),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(18, 0, 0)
            });

            var list = await _rotaService.GetRotasForStoreAsync(2);
            _output.WriteLine($"Total rotas for store 2: {list.Count()}");
            Assert.Single(list);
        }

        [Fact]
        public async Task GetRotasForUserAsync_ReturnsUserRotas()
        {
            await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 12),
                StartTime = new TimeSpan(11, 0, 0),
                EndTime = new TimeSpan(19, 0, 0)
            });

            var list = await _rotaService.GetRotasForUserAsync("u123");
            _output.WriteLine($"Total rotas for user u123: {list.Count()}");
            Assert.True(list.Any());
        }

        [Fact]
        public async Task GetWeeklyRotasForStoreAsync_ReturnsFilteredWeekRotas()
        {
            var weekStart = new DateTime(2025, 5, 5);
            var weekEnd = weekStart.AddDays(5);
            await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = weekStart,
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(16, 0, 0)
            });

            var weekRotas = await _rotaService.GetWeeklyRotasForStoreAsync(2, weekStart, weekEnd);
            _output.WriteLine($"Weekly rotas found: {weekRotas.Count()}");
            Assert.Single(weekRotas);
        }

        [Fact]
        public async Task UpdateRotaAsync_UpdatesSuccessfully()
        {
            var rota = await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 15),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            });

            rota.StartTime = new TimeSpan(10, 0, 0);
            rota.EndTime = new TimeSpan(18, 0, 0);
            var updated = await _rotaService.UpdateRotaAsync(rota.RotaId, rota);

            _output.WriteLine($"Updated rota: {updated.StartTime} - {updated.EndTime}");
            Assert.Equal(new TimeSpan(10, 0, 0), updated.StartTime);
        }

        [Fact]
        public async Task DeleteRotaAsync_RemovesRota()
        {
            var rota = await _rotaService.CreateRotaAsync(new RotaDTO
            {
                UserId = "u123",
                StoreId = 2,
                Date = new DateTime(2025, 5, 20),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(17, 0, 0)
            });

            await _rotaService.DeleteRotaAsync(rota.RotaId);
            var fetched = await _rotaService.GetRotaByIdAsync(rota.RotaId);

            _output.WriteLine($"Deleted rota with ID: {rota.RotaId}. Still in DB? {(fetched == null ? "No ✅" : "Yes ❌")}");
            Assert.Null(fetched);
        }
    }
}
