using static luanvanthacsi.Data.Components.Enum;

namespace luanvanthacsi.Models
{
    public class EvaluationBoardDocx
    {
        public Data Content { get; set; }

        public class Data
        {
            public string DateForm { get; set; }
            public string StudentName { get; set; }
            public string DOB { get; set; }
            public string TopicName { get; set; }
            public string FacultyName { get; set; }
            public string FacultyCode { get; set; }
            public string InstructorName { get; set; }
            public string BoardTotal { get; set; }

            public List<EvaluationBoard> EvaluationBoards { get; set; }
            public List<EvaluationBoard> EvaluationBoardAll { get; set; }

        }

        public class EvaluationBoard
        {
            public string No { get; set; }
            public string Name { get; set; }
            public string WorkUnit { get; set; }
            public string Title { get; set; }
            public string Degree { get; set; }
            public string Price { get; set; }
            public virtual EvaluationRole EvaluationRole { get; set; }

        }

    }
}
