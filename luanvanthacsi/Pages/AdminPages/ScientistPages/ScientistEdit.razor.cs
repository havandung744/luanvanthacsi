using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using AntDesign;
using Microsoft.AspNetCore.Components.Web;
using luanvanthacsi.Data.Extentions;
using Tewr.Blazor.FileReader;
using Microsoft.JSInterop;
using OfficeOpenXml.Style.XmlAccess;
using static luanvanthacsi.Data.Components.Enum;
using AutoMapper;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Ultils;

namespace luanvanthacsi.Pages.AdminPages.ScientistPages
{
    public partial class ScientistEdit : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Inject] IFileReaderService FileReaderService { get; set; }
        [Parameter] public User CurrentUser { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Scientist> ValueChange { get; set; }
        ScientistEditModel EditModel { get; set; } = new ScientistEditModel();
        Form<ScientistEditModel> form;
        IFileReaderRef Reader;
        ElementReference inputTypeFileElement;
        string AttachFileName;
        ScientistEditModel oldEditmode { get; set; }
        string PathFolder => Path.Combine("scientist");
        bool uploadVisible;
        List<selectInUniversity> _selectInUniversity;
        string Id { get; set; }
        protected override void OnInitialized()
        {
            oldEditmode = EditModel;
            _selectInUniversity = new List<selectInUniversity>
            {
                new selectInUniversity {Value=0, Name="Ngoài trường"},
                new selectInUniversity {Value=1, Name="Trong trường"},
            };
        }
        public void LoadData(Scientist scientist)
        {
            EditModel = _mapper.Map<ScientistEditModel>(scientist);
            StateHasChanged();
        }

        public void UpdateScientist()
        {
            Scientist scientist = _mapper.Map<Scientist>(EditModel);
            scientist.FacultyId = CurrentUser.FacultyId;
            ValueChange.InvokeAsync(scientist);
        }

        private void OnFinish(EditContext editContext)
        {
            UpdateScientist();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //EditModel = new ScientistEditModel();
        }

        public void Close()
        {
            EditModel = new ScientistEditModel();
            StateHasChanged();
        }
        private async Task Reset(MouseEventArgs args)
        {
            if (EditModel.Id == null)
            {
                var newCode = EditModel.Code;
                EditModel = new ScientistEditModel();
                EditModel.Code = newCode;
            }
            else
            {
                Scientist oldEditmodel = await ScientistService.GetScientistByIdAsync(EditModel.Id);
                EditModel = _mapper.Map<ScientistEditModel>(oldEditmodel);
            }
            StateHasChanged();
        }

        public async Task ReadFile(MouseEventArgs e)
        {
            try
            {
                Reader = FileReaderService.CreateReference(inputTypeFileElement);
                var fileUpload = (await Reader.EnumerateFilesAsync()).FirstOrDefault();
                if (fileUpload != null)
                {
                    var fileUploadInfo = await fileUpload.ReadFileInfoAsync();
                    await using Stream stream = await fileUpload.OpenReadAsync();
                    string pathFolder = Path.Combine(PathFolder, EditModel.Id);
                    if (!Directory.Exists(pathFolder))
                    {
                        Directory.CreateDirectory(pathFolder);
                    }
                    string fileName = ObjectExtentions.GenerateGuid() + Path.GetExtension(fileUploadInfo.Name);
                    using var fileStream = new FileStream(Path.Combine(pathFolder, fileName), FileMode.Create, FileAccess.Write);
                    await stream.CopyToAsync(fileStream);
                    string attachFilePath = Path.Combine(Path.Combine(pathFolder, fileName));
                    EditModel.SetValue(nameof(EditModel.FileName), fileUploadInfo.Name);
                    EditModel.Change(nameof(EditModel.AttachFilePath), attachFilePath);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            CancelModal();
        }

        void CancelModal()
        {
            uploadVisible = false;
        }
        public void GetNameFile(ChangeEventArgs e)
        {
            AttachFileName = Path.GetFileName(e.Value?.ToString());
            StateHasChanged();
        }

        public void ClearFile(ScientistEditModel model = null)
        {
            try
            {
                if (model != null)
                {
                    model.AttachFilePath = null;
                    model.FileName = null;
                }
                if (Reader != null)
                {
                    Reader.ClearValue();
                }
                AttachFileName = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        void Upload(ScientistEditModel model)
        {
            try
            {
                ClearFile();
                uploadVisible = true;
                EditModel = model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task GetFileAttachAsync(ScientistEditModel model)
        {
            try
            {
                if (model.AttachFilePath.IsNullOrEmpty())
                {
                    return;
                }

                ////var filePathUrl = Path.Combine("luanvanthacsi", model.AttachFilePath, model.FileName);
                //string fileUrl = "file:///C:/chuongtrinhki1nam4/khoaluan/luanvanthacsi/luanvanthacsi/wwwroot/Documents/example.pdf"''
                //JSRuntime.DownloadFileFromUrl(fileUrl, EditModel.FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
