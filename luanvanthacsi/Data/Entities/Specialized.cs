﻿namespace luanvanthacsi.Data.Entities
{
    public class Specialized
    {
        public virtual string Id { get; set; }
        public virtual string? Code { get; set; }
        public virtual string? Name { get; set; }
        public virtual string? FacultyId { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
    }
}
