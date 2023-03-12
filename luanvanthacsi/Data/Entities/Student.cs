namespace luanvanthacsi.Data.Entities
{
    public class Student
    {
        public virtual string Id { get; set; }
        public virtual string? Code { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? Email { get; set; }
        public virtual string? FacultyId { get; set; }
        public virtual string? ThesisDefenseId { get; set; }
        public virtual string? PhoneNumber { get; set; }
        public virtual DateTime? DateOfBirth { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string? TopicName { get; set; }
        public virtual string? InstructorOne { get; set; }
        public virtual string? OnstructorTwo { get; set; }
    }
}
