﻿using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class StudentMap : ClassMap<Student>
    {
        public StudentMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            Map(x => x.Code).Column("Code");
            Map(x => x.Name).Column("Name");
            Map(x => x.Email).Column("Email");
            Map(x => x.PhoneNumber).Column("PhoneNumber");
            Map(x => x.DateOfBirth).Column("DateOfBirth");
            Map(x => x.CreateDate).Column("CreateDate");
            Map(x => x.UpdateDate).Column("UpdateDate");
            Map(x => x.FacultyId).Column("FacultyId");
            Table("Student");
        }

    }
}
