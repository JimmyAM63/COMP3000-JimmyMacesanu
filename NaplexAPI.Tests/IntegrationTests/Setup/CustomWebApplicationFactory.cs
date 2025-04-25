using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.Entities;
using System;
using System.Linq;

namespace IntegrationTests.Setup
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName;

        public CustomWebApplicationFactory(string? dbName = null)
        {
            _dbName = dbName ?? $"TestDb_{Guid.NewGuid()}";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

            builder.ConfigureServices(services =>
            {
                // Remove the app's real DB context
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Replace it with in-memory DB
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(_dbName));

                // Rebuild service provider to get scoped services
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;

                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var roleManager = scopedServices.GetRequiredService<RoleManager<IdentityRole>>();

                db.Database.EnsureCreated(); // 👈 Needed for roleManager to work

                SeedRoles(roleManager).GetAwaiter().GetResult();
                SeedStores(db);
                SeedUsers(db);
                SeedEmployeeStores(db);
                SeedRotas(db);
                SeedSKUs(db);
                SeedSales(db);
            });
        }

        private async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "Store Manager", "Sales Advisor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private void SeedStores(ApplicationDbContext context)
        {
            var stores = new List<Store>
            {
                new Store { Id = 1, StoreName = "Newton_Abbot" },
                new Store { Id = 2, StoreName = "Exmouth" }
            };

            foreach (var store in stores)
            {
                if (!context.Stores.Any(s => s.Id == store.Id))
                    context.Stores.Add(store);
            }

            context.SaveChanges();
        }

        private void SeedUsers(ApplicationDbContext context)
        {
            var users = new List<User>
            {   
                new User { Id = "u123", UserName = "testuser", Email = "test@rota.com", FirstName = "Test", LastName = "User", PhoneNumber = "1234567890" },
                new User { Id = "u456", UserName = "seconduser", Email = "second@rota.com", FirstName = "Jane", LastName = "Smith", PhoneNumber = "9876543210" }
            };

            foreach (var user in users)
            {
                if (!context.Users.Any(u => u.Id == user.Id))
                    context.Users.Add(user);
            }

            context.SaveChanges();
        }

        private void SeedEmployeeStores(ApplicationDbContext context)
        {
            var employeeLinks = new List<EmployeeStore>
            {
                new EmployeeStore { UserId = "u123", StoreId = 1 },
                new EmployeeStore { UserId = "u456", StoreId = 2 }
            };

            foreach (var link in employeeLinks)
            {
                if (!context.EmployeeStores.Any(es => es.UserId == link.UserId && es.StoreId == link.StoreId))
                    context.EmployeeStores.Add(link);
            }

            context.SaveChanges();
        }

        private void SeedRotas(ApplicationDbContext context)
        {
            var employeeStores = context.EmployeeStores
                .Include(es => es.User)
                .Include(es => es.Store)
                .ToList();

            var rotas = new List<ROTA>
            {
                new ROTA
                {
                    EmployeeStore = employeeStores.First(es => es.UserId == "u123" && es.StoreId == 1),
                    Date = new DateTime(2025, 5, 1),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    IsLeave = false,
                    IsOff = false
                },
                new ROTA
                {
                    EmployeeStore = employeeStores.First(es => es.UserId == "u456" && es.StoreId == 2),
                    Date = new DateTime(2025, 5, 2),
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(20, 0, 0),
                    IsLeave = false,
                    IsOff = false
                }
            };

            if (!context.ROTAs.Any())
            {
                context.ROTAs.AddRange(rotas);
                context.SaveChanges();
            }
        }

        private void SeedSKUs(ApplicationDbContext context)
        {
            var skus = new List<SKU>
            {
                new SKU
                {
                    SOC_Code = "SKU123",
                    Type = "Mobile",
                    Band = "A",
                    MAF_Inc_VAT = 29.99m,
                    ContractLength = 24,
                    Description = "Unlimited Minutes + 10GB",
                    Acq_Ret = "Acquisition",
                    Newton_Abbot = 10,
                    Exmouth = 8,
                    Exeter = 12,
                    Plymouth = 11
                },
                new SKU
                {
                    SOC_Code = "SKU456",
                    Type = "Broadband",
                    Band = "B",
                    MAF_Inc_VAT = 19.99m,
                    ContractLength = 18,
                    Description = "Superfast Fibre 36Mbps",
                    Acq_Ret = "Retention",
                    Newton_Abbot = 9,
                    Exmouth = 7,
                    Exeter = 10,
                    Plymouth = 8
                }
            };

            if (!context.SKUs.Any())
            {
                context.SKUs.AddRange(skus);
                context.SaveChanges();
            }
        }

        private void SeedSales(ApplicationDbContext context)
        {
            var employeeStores = context.EmployeeStores
                .Include(es => es.User)
                .Include(es => es.Store)
                .ToList();

            var sales = new List<Sale>
            {
                new Sale
                {
                    EmployeeStore = employeeStores.First(es => es.UserId == "u123" && es.StoreId == 1),
                    SKU = "SKU123",
                    OrderType = "Acquisition",
                    OrderNumber = "TEST123",
                    SaleDate = new DateTime(2025, 5, 1),
                    SaleTime = new TimeSpan(9, 0, 0),
                    IsAdditional = false,
                    IsDiscounted = false
                }      
            };

            if (!context.Sales.Any())
            {
                context.Sales.AddRange(sales);
                context.SaveChanges();
            }
        }

    }
}
