using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaplexAPI.Models.DTOs;
using NaplexAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaplexAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RotaController : ControllerBase
    {
        private readonly IRotaService _rotaService;

        public RotaController(IRotaService rotaService)
        {
            _rotaService = rotaService ?? throw new ArgumentNullException(nameof(rotaService));
        }

        // POST: api/rota
        [HttpPost]
        public async Task<ActionResult<RotaDTO>> CreateRota(RotaDTO rotaDto)
        {
            var newRota = await _rotaService.CreateRotaAsync(rotaDto);
            return CreatedAtAction(nameof(GetRotaById), new { rotaId = newRota.RotaId }, newRota);
        }

        // GET: api/rota/{rotaId}
        [HttpGet("{rotaId}")]
        public async Task<ActionResult<RotaDTO>> GetRotaById(int rotaId)
        {
            var rota = await _rotaService.GetRotaByIdAsync(rotaId);
            if (rota == null)
            {
                return NotFound();
            }
            return rota;
        }

        // GET: api/rota/store/{storeId}
        [HttpGet("store/{storeId}")]
        public async Task<ActionResult<IEnumerable<RotaDTO>>> GetRotasForStore(int storeId)
        {
            var rotas = await _rotaService.GetRotasForStoreAsync(storeId);
            return Ok(rotas);
        }

        // GET: api/rota/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RotaDTO>>> GetRotasForUser(string userId)
        {
            var rotas = await _rotaService.GetRotasForUserAsync(userId);
            return Ok(rotas);
        }

        // GET: api/rota/store/{storeId}/weekly?start={weekStartDate}&end={weekEndDate}
        [HttpGet("store/{storeId}/weekly")]
        public async Task<ActionResult<List<RotaDTO>>> GetWeeklyRotasForStore(int storeId, DateTime weekStartDate, DateTime weekEndDate)
        {
            var weeklyRotas = await _rotaService.GetWeeklyRotasForStoreAsync(storeId, weekStartDate, weekEndDate);
            return Ok(weeklyRotas);
        }

        // PUT: api/rota/{rotaId}
        [HttpPut("{rotaId}")]
        public async Task<IActionResult> UpdateRota(int rotaId, RotaDTO rotaDto)
        {
            try
            {
                await _rotaService.UpdateRotaAsync(rotaId, rotaDto);
                return NoContent();
            }
            catch (ApplicationException ex) when (ex.Message == "Rota not found.")
            {
                return NotFound();
            }
        }

        // DELETE: api/rota/{rotaId}
        [HttpDelete("{rotaId}")]
        public async Task<IActionResult> DeleteRota(int rotaId)
        {
            try
            {
                await _rotaService.DeleteRotaAsync(rotaId);
                return NoContent();
            }
            catch (ApplicationException ex) when (ex.Message == "Rota not found.")
            {
                return NotFound();
            }
        }
    }
}
