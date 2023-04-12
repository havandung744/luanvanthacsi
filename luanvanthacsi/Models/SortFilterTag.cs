namespace luanvanthacsi.Models
{
    public class SortFilterTag
    {
        public string FieldName { get; set; }
        public string FieldDisplayName { get; set; }
        public SortFilterTagType Type { get; set; }
        public string Value { get; set; }
        public string Direction { get; set; }
        public string DirectionDisplay { get => Direction == "ascend" ? "tăng dần" : Direction == "descend" ? "giảm dần" : ""; }
        public string Operator { get; set; }
        public string OperatorDisplay { get; set; }
        public string FilterText { get => $"{FieldDisplayName} {OperatorDisplay?.ToLower()} '{Value}'"; }
        public string SortText { get => $"{FieldDisplayName} {DirectionDisplay}"; }
        public int Index { get; set; }
    }

    public enum SortFilterTagType
    {
        Sort,
        Filter
    }
}
