using Xunit;
using Moq;
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
    public class SalesServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SalesService _salesService;
        private readonly ITestOutputHelper _output;

        public SalesServiceTests(ITestOutputHelper output)
        {
            _output = output;

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"SalesServiceTestDB_{Guid.NewGuid()}")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _salesService = new SalesService(_dbContext);

            _dbContext.SKUs.Add(new SKU
            {
                SOC_Code = "112736",     // ✅ required
                Acq_Ret = "Acquisition", // ✅ required
                Type = "HBB",            // ✅ required
                Band = "FTTP",           // ✅ required
                Description = "Test SKU",
                Newton_Abbot = 88.56m,
                Exmouth = 88.56m,
                Exeter = 100.00m,
                Plymouth = 88.56m
            });

            _dbContext.Users.Add(new User { Id = "u123", FirstName = "Jimmy" });
            _dbContext.Stores.Add(new Store { Id = 1, StoreName = "Exeter" });
            _dbContext.EmployeeStores.Add(new EmployeeStore
            {
                UserId = "u123",
                StoreId = 1
            });

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateSale_CreatesSuccessfully()
        {
            var saleDto = new SaleDTO
            {
                SKU = "112736",
                OrderType = "Acquisition",
                OrderNumber = "ORD100",
                IsAdditional = true,
                IsDiscounted = false,
                StoreId = 1,
                UserId = "u123",
                SaleDate = new DateTime(2025, 5, 1),
                SaleTime = new TimeSpan(14, 0, 0)
            };

            var result = await _salesService.CreateSale(saleDto);

            _output.WriteLine($"Created sale ID: {result.SaleId}, Revenue: {result.Revenue}, Type: {result.OrderType}, Order #: {result.OrderNumber}, Additional: {result.IsAdditional}, Discounted: {result.IsDiscounted}");
            Assert.Equal(100.00m, result.Revenue);
        }

        [Fact]
        public async Task GetSaleById_ReturnsCorrectSale()
        {
            var sale = await _salesService.CreateSale(new SaleDTO
            {
                SKU = "112736",
                OrderType = "Acquisition",
                OrderNumber = "ORD101",
                IsAdditional = true,
                IsDiscounted = false,
                StoreId = 1,
                UserId = "u123",
                SaleDate = new DateTime(2025, 5, 1),
                SaleTime = new TimeSpan(14, 0, 0)
            });

            var fetched = await _salesService.GetSaleById(sale.SaleId);

            _output.WriteLine($"Fetched sale ID: {fetched.SaleId}, SKU: {fetched.SKU}, Revenue: {fetched.Revenue}");
            Assert.NotNull(fetched);
            Assert.Equal(sale.SaleId, fetched.SaleId);
        }

        [Fact]
        public async Task GetAllSales_ReturnsSales()
        {
            await _salesService.CreateSale(new SaleDTO
            {
                SKU = "112736",
                OrderType = "Acquisition",
                OrderNumber = "ORD102",
                IsAdditional = false,
                IsDiscounted = false,
                StoreId = 1,
                UserId = "u123",
                SaleDate = DateTime.Now,
                SaleTime = new TimeSpan(12, 0, 0)
            });

            var result = await _salesService.GetAllSales();
            _output.WriteLine($"Total sales found: {result.Count()}");
            Assert.True(result.Any());
        }

        [Fact]
        public async Task UpdateSale_UpdatesCorrectly()
        {
            var created = await _salesService.CreateSale(new SaleDTO
            {
                SKU = "112736",
                OrderType = "Acquisition",
                OrderNumber = "ORD103",
                IsAdditional = false,
                IsDiscounted = false,
                StoreId = 1,
                UserId = "u123",
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(13, 0, 0)
            });

            var updatedDto = new SaleDTO
            {
                SKU = "112736",
                OrderType = "Retention",
                OrderNumber = "ORD103-UPDATED",
                IsAdditional = true,
                IsDiscounted = true,
                StoreId = 1,
                UserId = "u123",
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(15, 0, 0)
            };

            await _salesService.UpdateSale(created.SaleId, updatedDto);

            var fetched = await _salesService.GetSaleById(created.SaleId);

            _output.WriteLine($"Updated sale time: {fetched.SaleTime}, Type: {fetched.OrderType}, Order #: {fetched.OrderNumber}, Additional: {fetched.IsAdditional}, Discounted: {fetched.IsDiscounted}");
            Assert.Equal(new TimeSpan(15, 0, 0), fetched.SaleTime);
            Assert.True(fetched.IsAdditional);
            Assert.True(fetched.IsDiscounted);
        }

        [Fact]
        public async Task DeleteSale_DeletesSuccessfully()
        {
            var sale = await _salesService.CreateSale(new SaleDTO
            {
                SKU = "112736",
                OrderType = "Acquisition",
                OrderNumber = "ORD104",
                IsAdditional = false,
                IsDiscounted = false,
                StoreId = 1,
                UserId = "u123",
                SaleDate = DateTime.Today,
                SaleTime = new TimeSpan(10, 0, 0)
            });

            await _salesService.DeleteSale(sale.SaleId);
            var fetched = await _salesService.GetSaleById(sale.SaleId);

            _output.WriteLine($"Deleted sale still exists? {(fetched == null ? "No ✅" : "Yes ❌")}");
            Assert.Null(fetched);
        }
    }
}
