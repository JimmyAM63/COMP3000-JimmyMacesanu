using NaplexAPI.Infrastructure;
using NaplexAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace NaplexAPI.Services
{
    public class StoreService : IStoreService
    {
        private readonly ApplicationDbContext _context; // Your database context

        public StoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StoreDTO>> GetAllStoresAsync()
        {
            return await _context.Stores
                .Select(s => new StoreDTO
                {
                    Id = s.Id,
                    StoreName = s.StoreName,
                    // Map other fields
                })
                .ToListAsync();
        }

        public async Task<StoreDTO> GetStoreByIdAsync(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null) return null;

            return new StoreDTO
            {
                Id = store.Id,
                StoreName = store.StoreName,
                // Map other fields
            };
        }

        public async Task<IEnumerable<StoreDTO>> GetStoresForUserAsync(string userId)
        {
            return await _context.EmployeeStores
                .Where(es => es.UserId == userId)
                .Select(es => new StoreDTO
                {
                    Id = es.Store.Id,
                    StoreName = es.Store.StoreName
                    // ... any other StoreDTO properties you want to include
                })
                .ToListAsync();
        }
    }


}
