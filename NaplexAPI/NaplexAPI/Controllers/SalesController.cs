using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaplexAPI.Models.DTOs; // Adjust based on your actual Sales DTOs namespace
using NaplexAPI.Services; // Ensure this is where your ISalesService is located
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NaplexAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;

        public SalesController(ISalesService salesService)
        {
            _salesService = salesService ?? throw new ArgumentNullException(nameof(salesService));
        }

        [HttpPost]
        public async Task<ActionResult<SaleDTO>> CreateSale([FromBody] SaleDTO saleDto)
        {
            //var userId = User.Identity.Name; // or however you retrieve the user's ID
            try
            {
                var newSale = await _salesService.CreateSale(saleDto);
                return CreatedAtAction(nameof(GetSaleById), new { id = newSale.SaleId }, newSale);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest("A sale with the same Order Number already exists.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SaleDTO>> GetSaleById(int id)
        {
            var sale = await _salesService.GetSaleById(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleDTO>>> GetAllSales()
        {
            var sales = await _salesService.GetAllSales();
            return Ok(sales);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(int id, [FromBody] SaleDTO saleDto)
        {
            await _salesService.UpdateSale(id, saleDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            await _salesService.DeleteSale(id);
            return NoContent();
        }

    }
}
