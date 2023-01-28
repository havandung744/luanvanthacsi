using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class ScientistMap : ClassMap<Scientist>
    {
        public ScientistMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            Map(x => x.Code).Column("Code");
            Map(x => x.Name).Column("Name");
            Map(x => x.Email).Column("Email");
            Map(x => x.PhoneNumber).Column("PhoneNumber");
            Map(x => x.Cv).Column("Cv");
            Map(x => x.AcademicRank).Column("AcademicRank");
            Map(x => x.Degree).Column("Degree");
            Table("Scientist");

        }
    }
}
