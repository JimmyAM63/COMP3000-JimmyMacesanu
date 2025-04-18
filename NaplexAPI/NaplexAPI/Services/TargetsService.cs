using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace NaplexAPI.Services
{
    public class TargetsService : ITargetsService
    {
        private readonly ApplicationDbContext _context;

        public TargetsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TargetDTO>> GetTargetsByStoreAndMonth(int storeId, string monthYearString)
        {
            DateTime monthYearDate = DateTime.ParseExact(monthYearString, "yyyy-MM", CultureInfo.InvariantCulture);
            var targets = await _context.Targets
                .Where(t => t.EmployeeStore.StoreId == storeId && t.TargetDate.Month == monthYearDate.Month && t.TargetDate.Year == monthYearDate.Year)
                .Include(t => t.EmployeeStore)
                .ThenInclude(es => es.Store)
                .Select(t => new TargetDTO
                {
                    TargetId = t.TargetId,
                    UserId = t.EmployeeStore.UserId, // Assuming conversion or handling string to int if necessary
                    StoreId = t.EmployeeStore.StoreId,
                    TargetDate = t.TargetDate,
                    NewTar = t.NewTar,
                    NewAct = t.NewAct,
                    TalkMobileAct = t.TalkMobileAct,
                    TalkMobileTar = t.TalkMobileTar,
                    UpgradesAct = t.UpgradesAct,
                    UpgradesTar = t.UpgradesTar,
                    HBBAct = t.HBBAct,
                    HBBTar = t.HBBTar,
                    HBBUpAct = t.HBBUpAct,
                    HBBUpTar = t.HBBUpTar, 
                    RevAct = t.RevAct,
                    RevTar = t.RevTar,
                    UnlimitedAct = t.UnlimitedAct,
                    UnlimitedTar = t.UnlimitedTar,
                    InsuranceAct = t.InsuranceAct,
                    InsuranceTar = t.InsuranceTar,
                    EntertainmentAct = t.EntertainmentAct,
                    EntertainmentTar = t.EntertainmentTar,
                    AdditionalAct = t.AdditionalAct,
                    AdditionalTar = t.AdditionalTar
                })
                .ToListAsync();

            return targets;
        }

        public async Task<IEnumerable<TargetDTO>> GetTargetsByUserAndMonth(string userId, string monthYearString)
        {

            DateTime monthYearDate = DateTime.ParseExact(monthYearString, "yyyy-MM", CultureInfo.InvariantCulture);

            var targets = await _context.Targets
                .Where(t => t.EmployeeStore.UserId == userId && t.TargetDate.Month == monthYearDate.Month && t.TargetDate.Year == monthYearDate.Year)
                .Include(t => t.EmployeeStore)
                .ThenInclude(es => es.User)
                .Select(t => new TargetDTO
                {
                    TargetId = t.TargetId,
                    UserId = t.EmployeeStore.UserId, // Assuming conversion or handling string to int if necessary
                    StoreId = t.EmployeeStore.StoreId,
                    TargetDate = t.TargetDate,
                    NewTar = t.NewTar,
                    NewAct = t.NewAct,
                    TalkMobileAct = t.TalkMobileAct,
                    TalkMobileTar = t.TalkMobileTar,
                    UpgradesAct = t.UpgradesAct,
                    UpgradesTar = t.UpgradesTar,
                    HBBAct = t.HBBAct,
                    HBBTar = t.HBBTar,
                    HBBUpAct = t.HBBUpAct,
                    HBBUpTar = t.HBBUpTar,
                    RevAct = t.RevAct,
                    RevTar = t.RevTar,
                    UnlimitedAct = t.UnlimitedAct,
                    UnlimitedTar = t.UnlimitedTar,
                    InsuranceAct = t.InsuranceAct,
                    InsuranceTar = t.InsuranceTar,
                    EntertainmentAct = t.EntertainmentAct,
                    EntertainmentTar = t.EntertainmentTar,
                    AdditionalAct = t.AdditionalAct,
                    AdditionalTar = t.AdditionalTar
                })
                .ToListAsync();

            return targets;
        }


        public async Task<TargetDTO> CreateTarget(TargetDTO targetDto)
        {
            // Adjusted to directly use es.StoreId for comparison instead of navigating to es.Store.StoreId
            var employeeStore = await _context.EmployeeStores
                .FirstOrDefaultAsync(es => es.UserId == targetDto.UserId && es.StoreId == targetDto.StoreId);

            if (employeeStore == null)
            {
                throw new ArgumentException("Invalid User ID or Store ID.");
            }

            var target = new Target
            {
                // Assuming EmployeeStore navigation property exists in Target and correctly set up
                EmployeeStore = employeeStore,
                TargetDate = targetDto.TargetDate,
                NewTar = targetDto.NewTar,
                NewAct = targetDto.NewAct,
                TalkMobileAct = targetDto.TalkMobileAct,
                TalkMobileTar = targetDto.TalkMobileTar,
                UpgradesAct = targetDto.UpgradesAct,
                UpgradesTar = targetDto.UpgradesTar,
                HBBAct = targetDto.HBBAct,
                HBBTar = targetDto.HBBTar,
                HBBUpAct = targetDto.HBBUpAct,
                HBBUpTar = targetDto.HBBUpTar,
                RevAct = targetDto.RevAct,
                RevTar = targetDto.RevTar,
                UnlimitedAct = targetDto.UnlimitedAct,
                UnlimitedTar = targetDto.UnlimitedTar,
                InsuranceAct = targetDto.InsuranceAct,
                InsuranceTar = targetDto.InsuranceTar,
                EntertainmentAct = targetDto.EntertainmentAct,
                EntertainmentTar = targetDto.EntertainmentTar,
                AdditionalAct = targetDto.AdditionalAct,
                AdditionalTar = targetDto.AdditionalTar
            };

            _context.Targets.Add(target);
            await _context.SaveChangesAsync();

            targetDto.TargetId = target.TargetId; // Populate the DTO with the newly created TargetId
            return targetDto;
        }



        public async Task UpdateTarget(int targetId, TargetDTO targetDto)
        {
            var target = await _context.Targets.FindAsync(targetId);

            if (target == null)
            {
                Console.WriteLine($"[DEBUG] Looking for EmployeeStore: UserId = '{targetDto.UserId}', StoreId = {targetDto.StoreId}");

                var employeeStore = await _context.EmployeeStores
                    .FirstOrDefaultAsync(es => es.UserId == targetDto.UserId && es.StoreId == targetDto.StoreId);

                if (employeeStore == null)
                    throw new ArgumentException("Invalid User ID or Store ID.");

                target = new Target
                {
                    EmployeeStore = employeeStore,
                    TargetDate = new DateTime(targetDto.TargetDate.Year, targetDto.TargetDate.Month, 1),
                    NewAct = 0,
                    TalkMobileAct = 0,
                    UpgradesAct = 0,
                    HBBAct = 0,
                    HBBUpAct = 0,
                    RevAct = 0,
                    UnlimitedAct = 0,
                    InsuranceAct = 0,
                    EntertainmentAct = 0,
                    AdditionalAct = 0
                };
                _context.Targets.Add(target);
            }

            // Apply requested targets
            target.NewTar = targetDto.NewTar;
            target.TalkMobileTar = targetDto.TalkMobileTar;
            target.UpgradesTar = targetDto.UpgradesTar;
            target.HBBTar = targetDto.HBBTar;
            target.HBBUpTar = targetDto.HBBUpTar;
            target.RevTar = targetDto.RevTar;
            target.UnlimitedTar = targetDto.UnlimitedTar;
            target.InsuranceTar = targetDto.InsuranceTar;
            target.EntertainmentTar = targetDto.EntertainmentTar;
            target.AdditionalTar = targetDto.AdditionalTar;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteTarget(int targetId)
        {
            var target = await _context.Targets.FindAsync(targetId);
            if (target == null)
            {
                throw new KeyNotFoundException("Target not found.");
            }

            _context.Targets.Remove(target);
            await _context.SaveChangesAsync();
        }

    }
}
