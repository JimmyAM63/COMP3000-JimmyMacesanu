namespace NaplexAPI.Models.Entities
{
    public class SKU
    {
        //public int ID { get; set; } // Auto-incremented primary key
        public string Type { get; set; }
        public string SOC_Code { get; set; } // Assuming SOC_Code is unique, this could also be a candidate for primary key if desired
        public string Band { get; set; }
        public decimal MAF_Inc_VAT { get; set; }
        public int ContractLength { get; set; }
        public string Description { get; set; }
        public string Acq_Ret { get; set; } // Assuming this stands for Acquisition/Retention
        public decimal Newton_Abbot { get; set; }
        public decimal Exmouth { get; set; }
        public decimal Exeter { get; set; }
        public decimal Plymouth { get; set; }
    }
}
