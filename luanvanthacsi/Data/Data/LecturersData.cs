using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class LecturersData
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
        [Display(Name = "Chức danh")]
        public string Title;
        [Display(Name = "Giới tính")]
        public string Gender;
    }
}
