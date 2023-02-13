using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class ThesisDefenseData
    {
        [Display(Name = "STT")]
        public int stt;
        public string Id;
        [Display(Name = "Mã")]
        public string Code;
        [Display(Name = "Tên đợt bảo vệ")]
        public string Name;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate;
    }
}
