using System.ComponentModel.DataAnnotations;
namespace luanvanthacsi.Data.Components
{
    public static class Enum
    {
        public enum locationOfScientist
        {
            [Display(Name = "Trong trường")]
            inUniInUniversity = 1,
            [Display(Name = "Ngoài trường")]
            notInUniInUniversity = 0,
        }
        public enum LoaiExcelImport
        {
            StaffProfile
        }

        public enum EvaluationRole
        {
            [Display(Name = "Chủ tịch")]
            President,
            [Display(Name = "Phản biện")]
            CounterAttack,
            [Display(Name = "Thư ký")]
            Secretary,
            [Display(Name = "Ủy viên")]
            Scientist,
            [Display(Name = "GVHD")]
            Instructor
        }

        public enum Status
        {
            Pedding = 1,
            Approved = 2,
            refuse = 3
        }

    }
}
