using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            Map(x => x.UserName).Column("UserName");
            Map(x => x.FacultyId).Column("FacultyId");
            Table("AspNetUsers");
        }
    }
}
