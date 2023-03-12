using AntDesign;
using luanvanthacsi.Data;
using luanvanthacsi.Data.Extentions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tewr.Blazor.FileReader;
using static luanvanthacsi.Data.Components.Enum;

namespace luanvanthacsi.Excel
{
    public partial class ImportExcelForm : ComponentBase
    {

        [Inject] IFileReaderService FileReaderService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] NotificationService Notice { get; set; }


        [Parameter] public string Title { get; set; } = "Nhập Excel file";
        [Parameter] public bool Visible { get; set; } = false;

        [Parameter] public EventCallback<(DataSet, string)> ImportSuccessStaff { get; set; }
        [Parameter] public EventCallback<bool> CancelChange { get; set; }
        [Parameter] public bool IsCustomTemplateFile { get; set; }
        [Parameter] public EventCallback CustomTemplateFileChange { get; set; }


        ElementReference sourceElement;
        IFileReaderRef Reader;
        public LoaiExcelImport LoaiExcelImport { get; set; }
        public List<ExcelSheetObject> Sheets { get; set; }

        public string DepartmentIdImport;
        public string FileMauUrl;
        string FileMauName;
        string NameFile;
        string Message;
        string MessageError;
        bool IsShowDetail;

        protected async override void OnInitialized()
        {

        }

        public async Task InitAsync()
        {
            await FileReaderService.CreateReference(sourceElement).ClearValue();
            NameFile = null;
            Message = null;
            MessageError = null;
        }

        async Task ReadFile()
        {
            //IsShowDetail = false;
            long max = 0;
            Message = string.Empty;
            Reader = FileReaderService.CreateReference(sourceElement);
            var files = (await Reader.EnumerateFilesAsync()).ToList();
            if (files.Count > 0)
            {

                foreach (var file in files)
                {
                    max += (await file.ReadFileInfoAsync()).Size;
                }
                if (max > 10 * 1024 * 1024)
                {
                    Message = "File quá lớn. Kích thước lớn nhất là 10MB.";
                    return;
                }
                var fileUpload = (await Reader.EnumerateFilesAsync()).FirstOrDefault();

                if (fileUpload != null)
                {
                    try
                    {
                        MessageError = null;
                        if (Sheets.IsNotNullOrEmpty())
                        {
                            foreach (var item in Sheets)
                            {
                                if (item.ErrorCells.IsNotNullOrEmpty())
                                {
                                    item.ErrorCells.Clear();
                                }

                                if (item.ErrorCellsTV.IsNotNullOrEmpty())
                                {
                                    item.ErrorCellsTV.Clear();
                                }
                            }
                        }
                        Message = "Đang tải tệp tin lên hệ thống...";
                        StateHasChanged();
                        var fileInRam = await fileUpload.CreateMemoryStreamAsync(4096);
                        var excel = new ExcelImporter(NameFile, "");
                        switch (LoaiExcelImport)
                        {
                            case LoaiExcelImport.StaffProfile:
                                Dictionary<string, List<KeyValuePair<int, int>>> errorCells = new Dictionary<string, List<KeyValuePair<int, int>>>();
                                errorCells.Clear();
                                using (var dtExcel = excel.ReadHsnsXlsx(Sheets, fileInRam, errorCells))
                                {
                                    if (errorCells?.Any() == true)
                                    {
                                        int i = 1;
                                        StringBuilder sb = new StringBuilder();
                                        foreach (var sheet in errorCells)
                                        {
                                            sb.Append($"Sheet: " + sheet.Key + "<br />");
                                            foreach (var group in sheet.Value.GroupBy(c => c.Key))
                                            {
                                                foreach (var item in group)
                                                {
                                                    sb.Append(string.Format(" [{0} - {1}] ", item.Key, (CellExcelEpplus)item.Value));
                                                }
                                                sb.Append("<br />");
                                            }
                                        }
                                        MessageError = sb.ToString();
                                        Message = "Dữ liệu trong tệp tin mẫu không đúng định dạng.";
                                    }
                                    else
                                    {
                                        if (dtExcel != null)
                                        {
                                            await ImportSuccessStaff.InvokeAsync((dtExcel, DepartmentIdImport));
                                            Cancel();
                                        }
                                        else
                                        {
                                            Message = "Không thể đọc dữ liệu";
                                        }
                                    }
                                }


                                break;



                        }
                        await ClearFile();
                    }
                    catch (Exception e)
                    {
                        //Message = e.ToString();
                        Message = "File mẫu không đúng định dạng. Vui lòng tải lại file mẫu mới nhất trên hệ thống và thử lại";
                        await ClearFile();
                    }
                }

            }
            else
            {
                Message = "Chưa chọn File";
            }
        }

        void Cancel()
        {
            CancelChange.InvokeAsync(false);
            IsShowDetail = false;
        }

        public void getNameFile(ChangeEventArgs e)
        {
            NameFile = Path.GetFileName(e.Value?.ToString());
            StateHasChanged();
        }
        async Task ClearFile()
        {
            if (Message != "")
            {
                await FileReaderService.CreateReference(sourceElement).ClearValue();
                NameFile = null;
            }
        }
        public async Task TaiFileMau()
        {
            if (IsCustomTemplateFile)
            {
                await CustomTemplateFileChange.InvokeAsync();
            }
            else
            {
                if (FileMauName.IsNullOrEmpty())
                {
                    FileMauName = FileMauUrl.Split('/').Last();
                }
                await JSRuntime.InvokeVoidAsync("downloadFile", FileMauUrl, FileMauUrl, FileMauName, "GET");
            }
        }
        public void ShowDetail()
        {
            IsShowDetail = !IsShowDetail;
        }
    }
}
