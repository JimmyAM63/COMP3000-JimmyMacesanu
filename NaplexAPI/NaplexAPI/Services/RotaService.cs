using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Models.Entities;

namespace NaplexAPI.Services
{
    public class RotaService : IRotaService
    {
        private readonly ApplicationDbContext _context;

        public RotaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RotaDTO> CreateRotaAsync(RotaDTO rotaDto)
        {
            // Check if the EmployeeStore relation exists
            var employeeStore = await _context.EmployeeStores
                .FirstOrDefaultAsync(es => es.UserId == rotaDto.UserId && es.StoreId == rotaDto.StoreId);

            if (employeeStore == null)
            {
                throw new ApplicationException("Invalid UserId or StoreId.");
            }

            bool exists = await _context.ROTAs
                .Include(r => r.EmployeeStore)
                .AnyAsync(r =>
                    r.EmployeeStore.UserId == rotaDto.UserId &&
                    r.EmployeeStore.StoreId == rotaDto.StoreId &&
                    r.Date == rotaDto.Date);

            if (exists)
                throw new ApplicationException("A rota already exists for this user on the selected date.");

            // Create new ROTA entity and set its properties from the DTO
            var rota = new ROTA
            {
                EmployeeStore = employeeStore,
                Date = rotaDto.Date.Date,
                StartTime = rotaDto.StartTime,
                EndTime = rotaDto.EndTime,
                IsLeave = rotaDto.IsLeave,
                IsOff = rotaDto.IsOff
            };

            _context.ROTAs.Add(rota);
            await _context.SaveChangesAsync();

            rotaDto.RotaId = rota.RotaId; // Assign the generated ID back to DTO
            return rotaDto;
        }

        public async Task<RotaDTO> GetRotaByIdAsync(int rotaId)
        {
            var rota = await _context.ROTAs
                .Include(r => r.EmployeeStore)
                .ThenInclude(es => es.Store)
                .Include(r => r.EmployeeStore)
                .ThenInclude(es => es.User)
                .FirstOrDefaultAsync(r => r.RotaId == rotaId);

            if (rota == null) return null;

            // Convert the rota to a DTO object
            return new RotaDTO
            {
                RotaId = rota.RotaId,
                UserId = rota.EmployeeStore.UserId,
                StoreId = rota.EmployeeStore.StoreId,
                Date = rota.Date,
                StartTime = rota.StartTime,
                EndTime = rota.EndTime,
                IsLeave = rota.IsLeave,
                IsOff = rota.IsOff
            };
        }

        public async Task<IEnumerable<RotaDTO>> GetRotasForStoreAsync(int storeId)
        {
            // Retrieve rotas related to the specified store
            var rotas = await _context.ROTAs
                .Include(r => r.EmployeeStore)
                .Where(r => r.EmployeeStore.StoreId == storeId)
                .ToListAsync();

            // Convert to DTOs
            return rotas.Select(r => new RotaDTO
            {
                RotaId = r.RotaId,
                UserId = r.EmployeeStore.UserId,
                StoreId = r.EmployeeStore.StoreId,
                Date = r.Date,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                IsLeave = r.IsLeave,
                IsOff = r.IsOff
            }).ToList();
        }

        public async Task<IEnumerable<RotaDTO>> GetRotasForUserAsync(string userId)
        {
            // Retrieve rotas related to the specified user
            var rotas = await _context.ROTAs
                .Include(r => r.EmployeeStore)
                .Where(r => r.EmployeeStore.UserId == userId)
                .ToListAsync();

            // Convert to DTOs
            return rotas.Select(r => new RotaDTO
            {
                RotaId = r.RotaId,
                UserId = r.EmployeeStore.UserId,
                StoreId = r.EmployeeStore.StoreId,
                Date = r.Date,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                IsLeave = r.IsLeave,
                IsOff = r.IsOff
            }).ToList();
        }

        public async Task<List<RotaDTO>> GetWeeklyRotasForStoreAsync(int storeId, DateTime weekStartDate, DateTime weekEndDate)
        {
            // Ensure dates are at the start/end of the day for accurate range comparison
            weekStartDate = weekStartDate.Date;
            weekEndDate = weekEndDate.Date.AddDays(1).AddTicks(-1);

            // Fetching the rotas within the specified date range for a particular store
            var rotas = await _context.ROTAs
                .Include(r => r.EmployeeStore)
                .Where(r => r.EmployeeStore.StoreId == storeId &&
                            r.Date >= weekStartDate &&
                            r.Date <= weekEndDate)
                .ToListAsync();

            // Mapping entities to DTOs
            var rotaDtos = rotas.Select(r => new RotaDTO
            {
                RotaId = r.RotaId,
                UserId = r.EmployeeStore.UserId,
                StoreId = r.EmployeeStore.StoreId,
                Date = r.Date,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                IsLeave = r.IsLeave,
                IsOff = r.IsOff
            }).ToList();

            return rotaDtos;
        }

        public async Task<RotaDTO> UpdateRotaAsync(int rotaId, RotaDTO rotaDto)
        {
            var existingRota = await _context.ROTAs.FindAsync(rotaId);
            if (existingRota == null)
            {
                throw new ApplicationException("Rota not found.");
            }

            // Update the properties
            existingRota.Date = rotaDto.Date;
            existingRota.StartTime = rotaDto.StartTime;
            existingRota.EndTime = rotaDto.EndTime;
            existingRota.IsLeave = rotaDto.IsLeave;
            existingRota.IsOff = rotaDto.IsOff;

            // You may also need to handle changes in the EmployeeStore association if necessary

            _context.ROTAs.Update(existingRota);
            await _context.SaveChangesAsync();

            return rotaDto;
        }

        public async Task DeleteRotaAsync(int rotaId)
        {
            var rota = await _context.ROTAs.FindAsync(rotaId);
            if (rota == null)
            {
                throw new ApplicationException("Rota not found.");
            }

            _context.ROTAs.Remove(rota);
            await _context.SaveChangesAsync();
        }
    }
}
