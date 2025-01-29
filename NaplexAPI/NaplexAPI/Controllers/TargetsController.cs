using Microsoft.AspNetCore.Mvc;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Services;
using System.Threading.Tasks;

namespace NaplexAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TargetsController : ControllerBase
    {
        private readonly ITargetsService _targetsService;

        public TargetsController(ITargetsService targetsService)
        {
            _targetsService = targetsService;
        }

        [HttpGet("store/{storeId}/{monthYear}")]
        public async Task<IActionResult> GetTargetsByStoreAndMonth(int storeId, string monthYear)
        {
            var targets = await _targetsService.GetTargetsByStoreAndMonth(storeId, monthYear);
            return Ok(targets);
        }

        [HttpGet("user/{userId}/{monthYear}")]
        public async Task<IActionResult> GetTargetsByUserAndMonth(string userId, string monthYear)
        {
            var targets = await _targetsService.GetTargetsByUserAndMonth(userId, monthYear);
            return Ok(targets);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTarget([FromBody] TargetDTO targetDto)
        {
            var createdTarget = await _targetsService.CreateTarget(targetDto);
            return CreatedAtAction(nameof(GetTargetsByUserAndMonth), new { userId = createdTarget.UserId, monthYear = createdTarget.TargetDate }, createdTarget);
        }

        [HttpPut("{targetId}")]
        public async Task<IActionResult> UpdateTarget(int targetId, [FromBody] TargetDTO targetDto)
        {
            await _targetsService.UpdateTarget(targetId, targetDto);
            return NoContent();
        }

        [HttpDelete("{targetId}")]
        public async Task<IActionResult> DeleteTarget(int targetId)
        {
            await _targetsService.DeleteTarget(targetId);
            return NoContent();
        }
    }
}
