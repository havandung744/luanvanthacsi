using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using luanvanthacsi.Models;
using Microsoft.JSInterop;

namespace luanvanthacsi.Ultils
{
    public class BrowserService
    {
        private readonly IJSRuntime _js;
        public event Action<BrowserDimension> WindowSizeChanged;

        public BrowserService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task<BrowserDimension> GetDimensions()
        {
            return await _js.InvokeAsync<BrowserDimension>("getDimensions");
        }

        [JSInvokable("BrowserDimensionChanged")]
        public void BrowserDimensionChanged(BrowserDimension dimension)
        {
            WindowSizeChanged?.Invoke(dimension);
        }

        public async Task BindBrowserDimensionChange(DotNetObjectReference<BrowserService> browserServiceRef)
        {
            await _js.InvokeVoidAsync("bindBrowserDimensionChange", browserServiceRef, "BrowserDimensionChanged");
        }
    }


}
