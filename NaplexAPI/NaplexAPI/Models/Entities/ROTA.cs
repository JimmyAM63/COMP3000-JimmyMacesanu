using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NaplexAPI.Models.Entities
{
    public class ROTA
    {
        [Key]
        public int RotaId { get; set; }
        public virtual EmployeeStore EmployeeStore { get; set; }
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public bool IsLeave { get; set; }
        public bool IsOff { get; set; }

        
    }
}
