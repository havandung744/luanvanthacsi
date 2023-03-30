using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Data
{
    public class EvaluationBoardData
    {
        [Display(Name = "STT")]
        public int stt;
        public string Id;
        [Display(Name = "Ngày tạo")]
        public DateTime CreateDate;
        [Display(Name = "Ngày Cập nhật")]
        public DateTime UpdateDate;
        [Display(Name = "Id học viên")]
        public string? StudentId;
        [Display(Name = "Học viên")]
        public string? StudentName;
        public DateTime? DOB;
        [Display(Name = "GVHD 1")]
        public string InstructorIdOne;
        [Display(Name = "GVHD 2")]
        public string InstructorIdTwo;
        [Display(Name = "Id khoa")]
        public string? FacultyId;
        [Display(Name = "Id chủ tịch")]
        public string? PresidentId;
        [Display(Name = "Chủ tịch")]
        public string? PresidentName;
        [Display(Name = "Id phản biện 1")]
        public string? CounterattackerIdOne;
        [Display(Name = "Phản Biện 1")]
        public string? CounterattackerOne;
        [Display(Name = "Phản biện 2")]
        public string? CounterattackerTwo;
        [Display(Name = "Phản biện 3")]
        public string? CounterattackerThree;
        [Display(Name = "Id phản biện 2")]
        public string? CounterattackerIdTwo;
        [Display(Name = "Id phản biện 3")]
        public string? CounterattackerIdThree;
        [Display(Name = "Id nhà khoa học 1")]
        public string? ScientistIdOne;
        [Display(Name = "Id nhà khoa học 2")]
        public string? ScientistIdTwo;
        [Display(Name = "Ủy viên 1")]
        public string? ScientistOne;
        [Display(Name = "Ủy viên 2")]
        public string? ScientistTwo;
        [Display(Name = "Id thư ký")]
        public string? SecretaryId;
        [Display(Name = "Thư ký")]
        public string? SecretaryName;
        [Display(Name = "Tên đề tài")]
        public string? TopicName;
        [Display(Name = "Chuyên ngành")]
        public string Branch;
        [Display(Name = "Hướng dẫn 1")]
        public string InstructorNameOne;
        [Display(Name = "Hướng dẫn 2")]
        public string InstructorNameTwo;
        [Display(Name = "Trạng thái")]
        public int Status;

    }
}
