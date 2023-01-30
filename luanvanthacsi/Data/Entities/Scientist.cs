using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Entities
{
    public class Scientist
    {
        public virtual string Id { get; set; }
        public virtual string? Code { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? PhoneNumber { get; set; }
        public virtual string? Cv { get; set; }
        public virtual string? AcademicRank { get; set; }
        public virtual string? Degree { get; set; }
    }
}
