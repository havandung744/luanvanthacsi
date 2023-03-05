using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class SecretaryMap : ClassMap<Secretary>
    {
        public SecretaryMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            Map(x => x.Code).Column("Code");
            Map(x => x.Name).Column("Name");
            Map(x => x.CreateDate).Column("CreateDate");
            Map(x => x.UpdateDate).Column("UpdateDate");
            Map(x => x.FacultyId).Column("FacultyId");
            Table("Secretary");
        }
    }
}
