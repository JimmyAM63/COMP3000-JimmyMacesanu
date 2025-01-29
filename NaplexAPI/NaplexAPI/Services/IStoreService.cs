using NaplexAPI.Models.DTOs;

namespace NaplexAPI.Services
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreDTO>> GetAllStoresAsync();
        Task<StoreDTO> GetStoreByIdAsync(int id);
        Task<IEnumerable<StoreDTO>> GetStoresForUserAsync(string userId);
    }

}
