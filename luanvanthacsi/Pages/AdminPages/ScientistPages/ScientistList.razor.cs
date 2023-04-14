using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace luanvanthacsi.Pages.AdminPages.ScientistPages
{
    public partial class ScientistList : ComponentBase
    {
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] ISpecializedService SpecializedService { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] IFacultyService FacultyService { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [CascadingParameter] SessionData SessionData { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        List<Scientist>? scientists { get; set; }
        bool visible = false;
        ScientistEdit scientistEdit = new ScientistEdit();
        bool loading = false;
        IEnumerable<ScientistData>? selectedRows;
        ScientistData? selectData;
        Table<ScientistData>? table;
        List<string>? ListSelectedScientistIds;
        List<Faculty> facultyList { get; set; }
        List<Specialized> specializedList { get; set; }
        string facultyId;
        string value;
        TableFilterTagRow<ScientistData> tableFilterTagRow;

        protected override async Task OnInitializedAsync()
        {
            scientistDatas = new();
            facultyList = await FacultyService.GetAllAsync();
            specializedList = await SpecializedService.GetAllAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAsync();
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                loading = true;
                StateHasChanged();
                visible = false;
                scientistDatas?.Clear();
                List<Scientist> scientists = new List<Scientist>();
                if (SessionData.CurrentUser?.FacultyId == null)
                {
                    facultyId = await localStorage.GetItemAsync<string>("facultyIdOfScientist");
                    scientists = await ScientistService.GetAllByIdAsync(facultyId);
                }
                else
                {
                    scientists = await ScientistService.GetAllByIdAsync(SessionData.CurrentUser.FacultyId);
                }
                // hiển thị dữ liệu mới nhất lên đầu trang
                var list = scientists.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
                foreach (Scientist item in list)
                {
                    Specialized specializedObj = specializedList?.FirstOrDefault(s => s.Id == item.SpecializedId);
                    if (specializedObj != null)
                    {
                        item.SpecializedName = specializedObj?.Name;
                    }
                }
                scientistDatas = _mapper.Map<List<ScientistData>>(list);
                int stt = 1;
                scientistDatas.ForEach(x => { x.stt = stt++; });
                loading = false;
                StateHasChanged();
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task AddScientist()
        {
            var scientistData = new Scientist();
            var lastCode = scientistDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 1;
            if (lastCode != null && int.TryParse(lastCode.Substring(3), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "NKH" + codeNumber.ToString("D3");
            scientistData.Code = newCode;
            await ShowScientistDetail(scientistData);
        }

        async Task ShowScientistDetail(Scientist data)
        {
            await scientistEdit.LoadData(data);
            visible = true;
        }

        async Task Save(Scientist data)
        {
            string check = data.Id;
            var resultAdd = await ScientistService.AddOrUpdateScientist(data);
            if (resultAdd == true)
            {
                if (check.IsNotNullOrEmpty())
                {
                    Notice.NotiSuccess("Cập nhật dữ liệu thành công.");
                }
                else
                {
                    Notice.NotiSuccess("Thêm dữ liệu thành công.");
                }
                await LoadAsync();
            }
            else
            {
                if (check.IsNotNullOrEmpty())
                {
                    Notice.NotiSuccess("Cập nhật dữ liệu thất bại.");
                }
                else
                {
                    Notice.NotiSuccess("Thêm dữ liệu thất bại.");
                }
            }

        }

        void OnClose()
        {
            scientistEdit.Close();
            visible = false;
        }

        async Task Edit(ScientistData scientistData)
        {
            Scientist scientis = await ScientistService.GetScientistByIdAsync(scientistData.Id);
            await ShowScientistDetail(scientis);
        }

        async Task DeleteScientist(ScientistData scientistData)
        {
            Scientist scientist = await ScientistService.GetScientistByIdAsync(scientistData.Id.ToString());
            var result = await ScientistService.DeleteScientistAsync(scientist);
            if (result.Equals(true))
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công.");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại.");
            }
        }

        void OnRowClick(RowData<ScientistData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = scientistDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
                ids = selectedRows != null ? selectedRows.Select(c => c.Id).ToList() : new();
                if (ids.Contains(selectData.Id))
                {
                    ids.Remove(selectData.Id);
                }
                else
                {
                    ids.Add(selectData.Id);
                }
                table?.SetSelection(ids.ToArray());
                ListSelectedScientistIds = ids;
            }
            catch (Exception)
            {
                throw;
            }
        }
        async Task DeleteAsync(ScientistData model = null)
        {
            try
            {
                var deleteModel = scientistDatas?.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<Scientist> scientists = new List<Scientist>();
                scientists = _mapper.Map<List<Scientist>>(deleteModel);
                var result = await ScientistService.DeleteScientistListAsync(scientists);
                if (result.Equals(true))
                {
                    Notice.NotiSuccess("Xóa dữ liệu thành công.");
                    await LoadAsync();
                }
                else
                {
                    Notice.NotiError("Xóa dữ liệu thất bại.");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
        async Task ChangeFacultyId()
        {
            await localStorage.SetItemAsync("facultyIdOfScientist", facultyId);
            await LoadAsync();
        }
    }
}
