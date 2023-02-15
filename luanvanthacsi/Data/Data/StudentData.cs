using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class StudentData
    {
        [Display(Name = "STT")]
        public int stt;
        public string Id;
        [Display(Name = "Mã")]
        public string Code;
        [Display(Name = "Họ tên")]
        public string Name;
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber;
        [Display(Name = "Email")]
        public string Email;
        [Display(Name = "Ngày sinh")]
        public DateTime DateOfBirth;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate;
        [Display(Name = "Ngày Cập nhật")]
        public DateTime UpdateDate;
        [Display(Name = "Mã khoa")]
        public string FacultyId;

    }
}
