using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class StudentEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào mã học viên")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào tên học viên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập vào email")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string FacultyId { get; set; }
        public string TopicName { get; set; }
        public string InstructorOne { get; set; }
        public string OnstructorTwo { get; set; }

    }
}
