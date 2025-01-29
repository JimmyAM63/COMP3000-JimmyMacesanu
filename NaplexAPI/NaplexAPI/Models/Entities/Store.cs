using System.ComponentModel.DataAnnotations;

namespace NaplexAPI.Models.Entities
{
    public class Store
    {
        [Key]
        public int Id { get; set; }
        public string? StoreName { get; set; }

        public ICollection<EmployeeStore>? EmployeeStores { get; set; }
    }
}
