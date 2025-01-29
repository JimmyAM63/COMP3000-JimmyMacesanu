using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NaplexAPI.Models.Entities
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }
        public string SKU { get; set; }
        public string OrderType { get; set; }
        public string OrderNumber { get; set; }
        public bool IsAdditional { get; set; }
        public bool IsDiscounted { get; set; }
        public virtual EmployeeStore EmployeeStore { get; set; }
        [Column(TypeName = "date")]
        public DateTime SaleDate { get; set; }
        public TimeSpan SaleTime { get; set; }
        public decimal Revenue { get; set; }
        public string? SKU_Type { get; set; }
        public string? SKU_Description { get; set; }
    }
}
