using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace luanvanthacsi.Data.Components
{
    public partial class Lazy : ComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }
        bool isLazyLoad;
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _ = Task.Run(() =>
                {
                    _ = InvokeAsync(() =>
                    {
                        isLazyLoad = true;
                        StateHasChanged();
                    });
                });
            }
        }
    }
}
