using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class ScientistMap : ClassMap<Scientist>
    {
        public ScientistMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            References(x => x.Specialized).Column("SpecializedId");
            Map(x => x.Code).Column("Code");
            Map(x => x.Name).Column("Name");
            Map(x => x.Email).Column("Email");
            Map(x => x.PhoneNumber).Column("PhoneNumber");
            Map(x => x.Cv).Column("Cv");
            Map(x => x.AcademicRank).Column("AcademicRank");
            Map(x => x.Degree).Column("Degree");
            Map(x => x.CreateDate).Column("CreateDate");
            Map(x => x.UpdateDate).Column("UpdateDate");
            Map(x => x.FacultyId).Column("FacultyId");
            Map(x => x.AttachFilePath).Column("AttachFilePath");
            Map(x => x.FileName).Column("FileName");
            Map(x => x.InUniversity).Column("InUniversity");
            Map(x => x.SpecializedName).Column("SpecializedName");
            Map(x => x.WorkingAgency).Column("WorkingAgency");
            Table("Scientist");
        }
    }
}
