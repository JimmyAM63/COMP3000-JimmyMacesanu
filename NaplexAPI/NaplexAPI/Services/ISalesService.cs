using NaplexAPI.Models.DTOs;

namespace NaplexAPI.Services
{
    public interface ISalesService
    {
        Task<SaleDTO> CreateSale(SaleDTO saleDto);
        Task<SaleDTO> GetSaleById(int saleId);
        Task<IEnumerable<SaleDTO>> GetAllSales();
        Task UpdateSale(int id, SaleDTO saleDto);
        Task DeleteSale(int id);
    }
}
