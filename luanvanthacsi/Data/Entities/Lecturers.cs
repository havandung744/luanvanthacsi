using AntDesign;

namespace luanvanthacsi.Data.Entities
{
    public class Lecturers
    {
        public virtual int Stt { get; set; }
        public virtual string Id { get; set; }
        public virtual string? Code { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? FacultyId { get; set; }
        public virtual string? PhoneNumber { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual int? Gender { get;set; }
        public virtual string? Title { get;set; }
    }
}
