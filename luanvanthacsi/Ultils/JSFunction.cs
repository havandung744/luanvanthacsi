using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace luanvanthacsi.Ultils
{
    public static class JSFunction
    {
        public static async Task<(int, string)> InsertTextToTextareaAsync(this IJSRuntime jSRuntime, string id, string text)
        {
            var data = await jSRuntime.InvokeAsync<System.Text.Json.JsonElement>("insertTextToTextarea", id, text);
            return (data.GetProperty("cursorPosition").GetInt32(), data.GetProperty("text").GetString());
        }

        public static void SetCursorPosition(this IJSRuntime jSRuntime, int position)
        {
            jSRuntime.InvokeVoidAsync("setCursorPosition", position);
        }

        public static void SaveAsFile(this IJSRuntime jSRuntime, string name, string base64)
        {
            jSRuntime.InvokeVoidAsync("saveAsFile", name, base64);
        }

        public static void DownloadFileFromUrl(this IJSRuntime jSRuntime, string url, string name)
        {
            jSRuntime.InvokeVoidAsync("downloadFile", url, url, name, HttpMethod.Get.ToString());
        }
    }
}
