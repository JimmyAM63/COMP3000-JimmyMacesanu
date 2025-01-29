using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NaplexAPI.Models.DTOs;

namespace NaplexAPI.Services
{
    public interface IRotaService
    {
        Task<RotaDTO> CreateRotaAsync(RotaDTO rotaDto);
        Task<RotaDTO> GetRotaByIdAsync(int rotaId);
        Task<IEnumerable<RotaDTO>> GetRotasForStoreAsync(int storeId);
        Task<IEnumerable<RotaDTO>> GetRotasForUserAsync(string userId);
        Task<List<RotaDTO>> GetWeeklyRotasForStoreAsync(int storeId, DateTime weekStartDate, DateTime weekEndDate);
        Task<RotaDTO> UpdateRotaAsync(int rotaId, RotaDTO rotaDto);
        Task DeleteRotaAsync(int rotaId);
    }
}

