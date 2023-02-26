using System.ComponentModel.DataAnnotations;

namespace luanvanthacsi.Data.Edit
{
    public class EvaluationBoardEditModel : EditBaseModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
