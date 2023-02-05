using AntDesign;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Data.Components
{
    public partial class PaginationCustom : ComponentBase
    {
        [Parameter]
        public PaginationSize Size { get; set; } = PaginationSize.Default;
        [Parameter]
        public (int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange) Context { get; set; }
    }
}
