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
    }
}
