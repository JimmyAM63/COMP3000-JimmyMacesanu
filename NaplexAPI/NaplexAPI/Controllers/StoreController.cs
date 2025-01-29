using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Services;

namespace NaplexAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        // GET: api/stores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoreDTO>>> GetStores()
        {
            var stores = await _storeService.GetAllStoresAsync();
            return Ok(stores);
        }

        // GET: api/stores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoreDTO>> GetStore(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return Ok(store);
        }

        [HttpGet("user/{userId}/stores")]
        public async Task<IActionResult> GetUserStores(string userId)
        {
            var stores = await _storeService.GetStoresForUserAsync(userId);
            return Ok(stores);
        }
    }

}
