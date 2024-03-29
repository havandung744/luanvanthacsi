﻿using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class StudentEditModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã học viên")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên học viên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [RegularExpression(@"^[\w-\._\+%]+@(?:[\w-]+\.)+[\w]{2,6}$", ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string FacultyId { get; set; }
        public string TopicName { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn hướng dẫn 1")]
        public string InstructorIdOne { get; set; }
        public string InstructorIdTwo { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn chuyên ngành")]
        public string SpecializedId { get; set; }

    }
}
