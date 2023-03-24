using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using System.Linq;
using AntDesign;
using luanvanthacsi.Data.Components;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Extentions;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components.Authorization;
using AutoMapper;

namespace luanvanthacsi.Pages.AdminPages.ScientistPages
{
    public partial class ScientistList : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        [Inject] IMapper _mapper { get; set; }
        List<Scientist>? scientists { get; set; }
        bool visible = false;
        ScientistEdit scientistEdit = new ScientistEdit();
        bool loading = false;
        IEnumerable<ScientistData>? selectedRows;
        ScientistData? selectData;
        Table<ScientistData>? table;
        List<string>? ListSelectedScientistIds;
        User CurrentUser;
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            scientistDatas = new();
            await LoadAsync();
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

        public async Task LoadAsync()
        {
            scientistDatas?.Clear();
            loading = true;
            visible = false;
            StateHasChanged();
            //var scientists = await ScientistService.GetAll();
            var scientists = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = scientists.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            scientistDatas = _mapper.Map<List<ScientistData>>(list);
            int stt = 1;
            scientistDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
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
            var resultAdd = await ScientistService.AddOrUpdateScientist(data);
            await LoadAsync();
            if (data.Id.IsNotNullOrEmpty())
            {
                Notice.NotiSuccess("Cập nhật dữ liệu thành công");
            }
            else
            {
                Notice.NotiSuccess("Thêm dữ liệu thành công");
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
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại");
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async Task DeleteAsync(ScientistData model = null)
        {
            try
            {
                var deleteModel = scientistDatas?.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<Scientist> scientists = new List<Scientist>();
                // maping từ studentData thành student
                foreach (var scientistData in deleteModel)
                {
                    Scientist scientist = new Scientist();
                    scientist.Id = scientistData.Id;
                    scientists.Add(scientist);
                }
                var result = await ScientistService.DeleteScientistListAsync(scientists);
                if (result.Equals(true))
                {
                    Notice.NotiSuccess("Xóa dữ liệu thành công");
                    await LoadAsync();
                }
                else
                {
                    Notice.NotiError("Xóa dữ liệu thất bại");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
