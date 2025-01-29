using Microsoft.AspNetCore.Mvc;
using NaplexAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaplexAPI.Services
{
    public interface ITargetsService
    {
        Task<IEnumerable<TargetDTO>> GetTargetsByStoreAndMonth(int storeId, string monthYear);
        Task<IEnumerable<TargetDTO>> GetTargetsByUserAndMonth(string userId, string monthYear);
        Task<TargetDTO> CreateTarget(TargetDTO targetDto);
        Task UpdateTarget(int targetId, TargetDTO targetDto);
        Task DeleteTarget(int targetId);
    }
}
