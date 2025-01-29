using System.ComponentModel.DataAnnotations.Schema;

namespace NaplexAPI.Models.DTOs
{
    public class TargetDTO
    {
        public int TargetId { get; set; }
        public string UserId { get; set; }
        public int StoreId { get; set; }
        [Column(TypeName = "date")]
        public DateTime TargetDate { get; set; }
        public int NewTar { get; set; }
        public int NewAct { get; set; }
        public int TalkMobileTar { get; set; }
        public int TalkMobileAct { get; set; }
        public int UpgradesTar { get; set; }
        public int UpgradesAct { get; set; }
        public int HBBTar { get; set; }
        public int HBBAct { get; set; }
        public int HBBUpTar { get; set; }
        public int HBBUpAct { get; set; }
        public int RevTar { get; set; }
        public int RevAct { get; set; }
        public int UnlimitedTar { get; set; }
        public int UnlimitedAct { get; set; }
        public int InsuranceTar { get; set; }
        public int InsuranceAct { get; set; }
        public int EntertainmentTar { get; set; }
        public int EntertainmentAct { get; set; }
        public int AdditionalTar { get; set; }
        public int AdditionalAct { get; set; }
    }
}
