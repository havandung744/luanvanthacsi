using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Ultils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Tewr.Blazor.FileReader;

namespace luanvanthacsi.Pages.AdminPages.ScientistPages
{
    public partial class ScientistEdit : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] ISpecializedService SpecializedService { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Inject] IFileReaderService FileReaderService { get; set; }
        User CurrentUser;
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Scientist> ValueChange { get; set; }
        [Parameter] public string facultyId { get; set; }
        ScientistEditModel EditModel { get; set; } = new ScientistEditModel();
        Form<ScientistEditModel> form;
        IFileReaderRef Reader;
        ElementReference inputTypeFileElement;
        string AttachFileName;
        ScientistEditModel oldEditmode { get; set; }
        string PathFolder => Path.Combine("scientist");
        bool uploadVisible;
        List<selectInUniversity> _selectInUniversity;
        List<selectAcademicRank> _selectAcademicRanks;
        string Id { get; set; }
        List<Specialized> specializedList { get; set; }
        string templateUrl;
        bool templateViewVisible;

        protected override async void OnInitialized()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);

            oldEditmode = EditModel;
            _selectInUniversity = new List<selectInUniversity>
            {
                new selectInUniversity {Value=0, Name="Ngoài trường"},
                new selectInUniversity {Value=1, Name="Trong trường"},
            };
            _selectAcademicRanks = new List<selectAcademicRank>
            {
                new selectAcademicRank {Value = 0, Name="Phó giáo sư"},
                new selectAcademicRank {Value = 1, Name="Giáo sư"},
                new selectAcademicRank {Value = -1, Name="Không"},
            };
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }
        public async Task LoadData(Scientist scientist)
        {
            EditModel = _mapper.Map<ScientistEditModel>(scientist);
            if (CurrentUser.FacultyId == null)
            {
                specializedList = await SpecializedService.GetAllByFacultyIdAsync(facultyId);
            }
            else
            {
                specializedList = await SpecializedService.GetAllByFacultyIdAsync(CurrentUser.FacultyId);
            }
            StateHasChanged();
        }

        public async Task UpdateScientist()
        {
            Scientist scientist = _mapper.Map<Scientist>(EditModel);
            if (scientist.InUniversity == 0)
            {
                scientist.SpecializedId = null;
            }
            if (CurrentUser.FacultyId == null)
            {
                scientist.FacultyId = facultyId;
            }
            else
            {
                scientist.FacultyId = CurrentUser.FacultyId;
            }
            await ValueChange.InvokeAsync(scientist);
        }

        private async void OnFinish(EditContext editContext)
        {
            await UpdateScientist();
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
                    string pathFolder = Path.Combine(PathFolder);
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
            catch (Exception)
            {
                throw;
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
                JSRuntime.DownloadFileFromUrl(model.AttachFilePath, model.FileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        async void ChangeSelected()
        {
            if (EditModel.InUniversity == 1)
            {
                EditModel.WorkingAgency = "Trường ĐHSP Hà Nội";
            }
            else
            {
                EditModel.SpecializedId = specializedList?.Select(x => x.Id).FirstOrDefault();
            }
        }

        private void OpenPdf()
        {
            try
            {
                if (EditModel.AttachFilePath.IsNotNullOrEmpty())
                {
                    templateViewVisible = true;
                    templateUrl = Path.Combine(EditModel.AttachFilePath);
                }
                else
                {
                    Notice.NotiWarning("Chưa Upload CV.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
