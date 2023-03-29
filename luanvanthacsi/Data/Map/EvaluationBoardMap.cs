using FluentNHibernate.Mapping;
using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Map
{
    public class EvaluationBoardMap : ClassMap<EvaluationBoard>
    {
        public EvaluationBoardMap()
        {
            Id(x => x.Id).GeneratedBy.Assigned().Column("Id");
            Map(x => x.Code).Column("Code");
            Map(x => x.Name).Column("Name");
            Map(x => x.CreateDate).Column("CreateDate");
            Map(x => x.UpdateDate).Column("UpdateDate");
            Map(x => x.StudentId).Column("StudentId");
            Map(x => x.FacultyId).Column("FacultyId");
            Map(x => x.PresidentId).Column("PresidentId");
            Map(x => x.CounterattackerIdOne).Column("CounterattackerIdOne");
            Map(x => x.CounterattackerIdTwo).Column("CounterattackerIdTwo");
            Map(x => x.CounterattackerIdThree).Column("CounterattackerIdThree");
            Map(x => x.ScientistIdOne).Column("ScientistIdOne");
            Map(x => x.ScientistIdTwo).Column("ScientistIdTwo");
            Map(x => x.SecretaryId).Column("SecretaryId");
            Map(x => x.Status).Column("Status");
            Table("EvaluationBoard");
        }
    }
}
