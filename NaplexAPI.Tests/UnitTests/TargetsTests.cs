using Xunit;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using NaplexAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;



namespace UnitTests
{
    public class TargetsServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TargetsService _targetsService;
        private readonly ITestOutputHelper _output;

        public TargetsServiceTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"TargetsServiceTestDB_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _targetsService = new TargetsService(_dbContext);

            var store = new Store { Id = 1, StoreName = "Exeter" };
            var user = new User { Id = "u123", FirstName = "Jimmy" };
            var es = new EmployeeStore { StoreId = store.Id, UserId = user.Id, Store = store, User = user };

            _dbContext.Stores.Add(store);
            _dbContext.Users.Add(user);
            _dbContext.EmployeeStores.Add(es);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateTarget_CreatesSuccessfully()
        {
            var targetDto = new TargetDTO
            {
                UserId = "u123",
                StoreId = 1,
                TargetDate = new DateTime(2025, 5, 1),
                NewTar = 10,
                TalkMobileTar = 5,
                UpgradesTar = 3,
                HBBTar = 2,
                HBBUpTar = 1,
                RevTar = 1000,
                UnlimitedTar = 4,
                InsuranceTar = 3,
                EntertainmentTar = 2,
                AdditionalTar = 6
            };

            var result = await _targetsService.CreateTarget(targetDto);

            _output.WriteLine($"Created Target ID: {result.TargetId}, Store: {result.StoreId}, User: {result.UserId}, Month: {result.TargetDate:yyyy-MM}");
            Assert.True(result.TargetId > 0);
        }

        [Fact]
        public async Task GetTargetsByStoreAndMonth_ReturnsCorrectResults()
        {
            await CreateTarget_CreatesSuccessfully();
            var results = await _targetsService.GetTargetsByStoreAndMonth(1, "2025-05");

            _output.WriteLine($"Targets for store 1 in 2025-05: {results.Count()}");
            Assert.Single(results);
        }

        [Fact]
        public async Task GetTargetsByUserAndMonth_ReturnsCorrectResults()
        {
            await CreateTarget_CreatesSuccessfully();
            var results = await _targetsService.GetTargetsByUserAndMonth("u123", "2025-05");

            _output.WriteLine($"Targets for user u123 in 2025-05: {results.Count()}");
            Assert.Single(results);
        }

        [Fact]
        public async Task UpdateTarget_UpdatesSuccessfully()
        {
            var created = await _targetsService.CreateTarget(new TargetDTO
            {
                UserId = "u123",
                StoreId = 1,
                TargetDate = new DateTime(2025, 5, 1),
                NewTar = 10,
                TalkMobileTar = 5,
                UpgradesTar = 3,
                HBBTar = 2,
                HBBUpTar = 1,
                RevTar = 1000,
                UnlimitedTar = 4,
                InsuranceTar = 3,
                EntertainmentTar = 2,
                AdditionalTar = 6
            });

            created.NewTar = 15;
            await _targetsService.UpdateTarget(created.TargetId, created);

            var updated = await _targetsService.GetTargetsByStoreAndMonth(1, "2025-05");
            var updatedTarget = updated.First();

            _output.WriteLine($"Updated NewTar: {updatedTarget.NewTar}");
            Assert.Equal(15, updatedTarget.NewTar);
        }

        [Fact]
        public async Task DeleteTarget_DeletesSuccessfully()
        {
            var created = await _targetsService.CreateTarget(new TargetDTO
            {
                UserId = "u123",
                StoreId = 1,
                TargetDate = new DateTime(2025, 5, 1),
                NewTar = 10,
                TalkMobileTar = 5,
                UpgradesTar = 3,
                HBBTar = 2,
                HBBUpTar = 1,
                RevTar = 1000,
                UnlimitedTar = 4,
                InsuranceTar = 3,
                EntertainmentTar = 2,
                AdditionalTar = 6
            });

            await _targetsService.DeleteTarget(created.TargetId);
            var targetsAfterDelete = await _targetsService.GetTargetsByStoreAndMonth(1, "2025-05");

            _output.WriteLine($"Targets after delete: {targetsAfterDelete.Count()}");
            Assert.Empty(targetsAfterDelete);
        }
    }
}
