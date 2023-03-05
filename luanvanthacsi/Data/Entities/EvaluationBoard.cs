namespace luanvanthacsi.Data.Entities
{
    public class EvaluationBoard
    {
        public virtual string Id { get; set; }
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime UpdateDate { get; set; }
        public virtual string FacultyId { get; set; }
        public virtual string StudentId { get; set; }
        // chủ tịch
        public virtual string PresidentId { get;set; }
        //Phản biện 1
        public virtual string CounterattackerIdOne { get; set; }
         //Phản biện 2
        public virtual string CounterattackerIdTwo { get; set; }
         //Phản biện 3
        public virtual string CounterattackerIdThree { get; set; }
         //Nhà khoa học 1
        public virtual string ScientistIdOne { get; set; }
         //Nhà khoa học 2
        public virtual string ScientistIdTwo { get; set; }
         //thư ký
        public virtual string SecretaryId { get; set; }

        
    }
}
