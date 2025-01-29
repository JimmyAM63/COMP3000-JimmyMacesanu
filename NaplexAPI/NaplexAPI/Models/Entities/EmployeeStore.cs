namespace NaplexAPI.Models.Entities
{
    public class EmployeeStore
    {
        public string UserId { get; set; }
        public int StoreId { get; set; }

        public bool IsPrimary { get; set; }

        public virtual User? User { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<ROTA> ROTAs { get; set; } = new List<ROTA>();
    }
}
