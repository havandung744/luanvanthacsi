using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Entities
{
    public class Scientist
    {
        public virtual string Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual string Email { get; set; }
        public virtual string? PhoneNumber { get; set; }
        public virtual string? Cv { get; set; }
        public virtual int AcademicRank { get; set; }
        public virtual string? Degree { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string FacultyId { get; set;}
        public virtual string AttachFilePath { get; set;}
        public virtual string FileName { get; set;}
        public virtual int InUniversity { get; set;}
        public virtual string SpecializedId { get; set;}
        public virtual string SpecializedName { get => Specialized?.Name; set { } }
        public virtual Specialized Specialized { get; set;}

    }
}
