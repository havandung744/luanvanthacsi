using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class ScientistEditModel : EditBaseModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Tên.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
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
        [Required(ErrorMessage = "Vui lòng chọn chuyên ngành.")]
        public string SpecializedId { get; set; }
        //[Required(ErrorMessage = "Vui lòng nhập đơn vị công tác")]
        public string WorkingAgency {get;set;}

    }
}
