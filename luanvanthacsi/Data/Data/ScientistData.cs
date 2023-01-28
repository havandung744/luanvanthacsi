using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class ScientistData
    {
        [Display(Name = "STT")]
        public int stt;
        public Guid Id;
        [Display(Name = "Mã")]
        public string Code;
        [Display(Name = "Họ tên")]
        public string Name;
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber;
        [Display(Name = "Email")]
        public string Email;
    }

}
