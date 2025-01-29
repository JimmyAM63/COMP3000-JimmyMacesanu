using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Models.Entities;
using System.Globalization;

namespace NaplexAPI.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<EmployeeStore> EmployeeStores { get; set; }
        public DbSet<ROTA> ROTAs { get; set; }
        public DbSet<SKU> SKUs { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Target> Targets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Always call this at the start

            // Then add your configurations:

            modelBuilder.Entity<EmployeeStore>()  // Use EmployeeStore instead of UserStore
                .HasKey(es => new { es.UserId, es.StoreId });

            modelBuilder.Entity<EmployeeStore>()  // Use EmployeeStore instead of UserStore
                .HasOne(es => es.User)
                .WithMany(u => u.EmployeeStores)  // Ensure you've named the navigation property EmployeeStores in the User entity.
                .HasForeignKey(es => es.UserId);

            modelBuilder.Entity<EmployeeStore>()  // Use EmployeeStore instead of UserStore
                .HasOne(es => es.Store)
                .WithMany(s => s.EmployeeStores)  // Ensure you've named the navigation property EmployeeStores in the Store entity.
                .HasForeignKey(es => es.StoreId);

            modelBuilder.Entity<ROTA>()
                .HasOne(r => r.EmployeeStore)
                .WithMany(es => es.ROTAs);

            modelBuilder.Entity<ROTA>()
                .Property(e => e.StartTime)
                .HasConversion(
                    v => v.ToString(@"hh\:mm"),
                    v => TimeSpan.ParseExact(v, "hh\\:mm", CultureInfo.InvariantCulture)
                );

            modelBuilder.Entity<ROTA>()
                .Property(e => e.EndTime)
                .HasConversion(
                    v => v.ToString(@"hh\:mm"),
                    v => TimeSpan.ParseExact(v, "hh\\:mm", CultureInfo.InvariantCulture)
                );

            modelBuilder.Entity<Sale>()
                .Property(e => e.SaleTime)
                .HasConversion(
                    v => v.ToString(@"hh\:mm"),
                    v => TimeSpan.ParseExact(v, "hh\\:mm", CultureInfo.InvariantCulture)
                );

            modelBuilder.Entity<SKU>(entity =>
            {
                // Define the table name if different from the class name
                entity.ToTable("SKUs");

                // Define the composite primary key
                entity.HasKey(e => new { e.SOC_Code, e.Acq_Ret });

                // Define auto-increment for the ID
                //entity.Property(e => e.ID).ValueGeneratedOnAdd();

                // Define lengths and types as necessary
                entity.Property(e => e.Type).HasMaxLength(255);
                entity.Property(e => e.SOC_Code).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Band).HasMaxLength(255);
                entity.Property(e => e.MAF_Inc_VAT).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Description).HasMaxLength(255);
                entity.Property(e => e.Acq_Ret).HasMaxLength(255);
                entity.Property(e => e.Newton_Abbot).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Exmouth).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Exeter).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Plymouth).HasColumnType("decimal(10,2)");

                // Add more configurations as needed
            });
        }
    }
}