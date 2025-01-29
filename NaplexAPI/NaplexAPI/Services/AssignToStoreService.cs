using NaplexAPI.Infrastructure;
using NaplexAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace NaplexAPI.Services
{
    public class AssignToStoreService : IAssignToStoreService
    {
        private readonly ApplicationDbContext _context;

        public AssignToStoreService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AssignAdditionalStoreToUserAsync(string userId, int storeId)
        {
            // Validate user existence
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new ApplicationException("User not found.");
            }

            // Validate store existence
            var storeExists = await _context.Stores.AnyAsync(s => s.Id == storeId);
            if (!storeExists)
            {
                throw new ApplicationException("Store not found.");
            }

            // Check if the user is already assigned to the store
            var alreadyAssigned = await _context.EmployeeStores.AnyAsync(es => es.UserId == userId && es.StoreId == storeId);
            if (alreadyAssigned)
            {
                throw new ApplicationException("User is already assigned to the specified store.");
            }

            // Assign the store to the user
            var employeeStore = new EmployeeStore
            {
                UserId = userId,
                StoreId = storeId,
                IsPrimary = false
            };

            _context.EmployeeStores.Add(employeeStore);
            await _context.SaveChangesAsync();
        }
    }

}
