using System.ComponentModel.DataAnnotations.Schema;

namespace NaplexAPI.Models.DTOs
{
    public class RotaDTO
    {
        public int RotaId { get; set; }
        public string UserId { get; set; }
        public int StoreId { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsLeave { get; set; }
        public bool IsOff { get; set; }
        // You might want to include some properties from EmployeeStore or just its ID, depending on your needs.
    }
}
