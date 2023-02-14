using luanvanthacsi.Data.Data;
using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class ThesisDefenseEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào mã đợt bảo vệ")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào tên đợt bảo vệ")]
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class StudentOfThesisDefenseEditModel
    {
        public virtual string Id { get; set; }
        public virtual string StudentId { get; set; }
        public int Stt { get; set; }
        [Display(Name = "mã nhân viên")]
        public virtual string Code { get; set; }
        [Display(Name = "Tên nhân viên")]
        public virtual string Name { get; set; }
        [Display(Name = "Ngày sinh")]
        public virtual string DateOfBirth { get; set; }
        [Display(Name = "Email")]
        public virtual string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        public virtual string PhoneNumber { get; set; }

        public virtual StudentData student { get; set; }
    }

}
