using luanvanthacsi.Data.Components;
using System.ComponentModel.DataAnnotations;
namespace luanvanthacsi.Data.Edit
{
    public class ScientistEditModel : EditBaseModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập Tên.")]
        public string Name { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập email.")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Email không hợp lệ.")]
        public string? Email { get; set; }
        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        //[RegularExpression(@"^0(1\d{9}|3\d{8}|5\d{8}|7\d{8}|8\d{8}|9\d{8})$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }
        public int AcademicRank { get; set; }
        public string Degree { get => "Tiến sĩ"; set { } }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        [Display(Name = "Đính kèm")]
        public string AttachFilePath { get; set; }
        [Display(Name = "Tên file")]
        public string FileName { get; set; }
        public int InUniversity { get; set; }
        [RequiredIf("InUniversity", 1), System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn chuyên ngành.")]
        public string SpecializedId { get; set; }
        [RequiredIf("InUniversity", 0), System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Vui lòng chọn cơ quan công tác.")]
        public string WorkingAgency { get; set; }
    }
}
