using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class ScientistData
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
        [Display(Name = "Học hàm")]
        public int AcademicRank;
        [Display(Name = "Học vị")]
        public string Degree;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate;
        [Display(Name = "Ngày Cập nhật")]
        public DateTime UpdateDate;
        [Display(Name = "Mã khoa")]
        public string FacultyId;
        public string AttachFilePath;
        [Display(Name = "File Name")]
        public string FileName;
        [Display(Name = "Vị trí")]
        public int InUniversity;
        public string SpecializedId;
        [Display(Name = "Chuyên nghành")]
        public string SpecializedName; 
        [Display(Name = "Cơ quan công tác")]
        public string WorkingAgency;
    }

}
