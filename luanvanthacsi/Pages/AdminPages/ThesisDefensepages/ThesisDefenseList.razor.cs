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
using AutoMapper;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using Microsoft.JSInterop;
using luanvanthacsi.Ultils;
using Color = System.Drawing.Color;
using BlazorInputFile;
using OneOf.Types;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Components.Authorization;
using luanvanthacsi.Data.Migrations;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseList : ComponentBase
    {
        [Inject] IMapper _mapper { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] IFileUpload fileUpload { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        List<ThesisDefenseData> thesisDefenseDatas { get; set; }
        ThesisDefenseEdit thesisDefenseEdit = new ThesisDefenseEdit();
        ThesisDefenseDetail ThesisDefenseDetail = new ThesisDefenseDetail();
        List<ThesisDefense> thesisDefenses { get; set; }
        IEnumerable<ThesisDefenseData>? selectRows;
        ThesisDefenseData selectData;
        Table<ThesisDefenseData>? table;
        List<string>? ListSelectedThesisDefenseIds;
        ThesisDefenseSearchModal thesisDefenseSearchModal;
        IEnumerable<ThesisDefenseData>? selectedRows;
        List<string>? ListSelectedIds;
        bool visible = false;
        bool visibleForDetail = false;
        bool loading = false;
        User CurrentUser;
        string titleOfThesisDefenseDetail;


        protected override async Task OnInitializedAsync()
        {
            // lấy thông tin User đang đăng nhập
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            thesisDefenseDatas = new();
            await LoadAsync();
        }


        public async Task LoadAsync()
        {
            thesisDefenseDatas.Clear();
            loading = true;
            visible = false;
            visibleForDetail = false;
            var thesisDefenses = await ThesisDefenseService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = thesisDefenses.OrderByDescending(x => x.CreateDate).ToList();
            thesisDefenseDatas = _mapper.Map<List<ThesisDefenseData>>(list);
            int stt = 1;
            thesisDefenseDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        void AddThesisDefense()
        {
            var thesisDefenseData = new ThesisDefense();
            var lastCode = thesisDefenseDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 0;
            if (lastCode != null && int.TryParse(lastCode.Substring(3), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "DBV" + codeNumber.ToString("D3");
            thesisDefenseData.Code = newCode;
            ShowStudentDetail(thesisDefenseData);
        }

        void ShowStudentDetail(ThesisDefense data)
        {
            thesisDefenseEdit.LoadData(data);
            visible = true;
        }

        async Task Save(ThesisDefense data)
        {
            var checkExistId = data.Id;
            var resultAdd = await ThesisDefenseService.AddOrUpdateThesisDefenseAsync(data);
            if (resultAdd)
            {
                await LoadAsync();
                if (checkExistId.IsNotNullOrEmpty())
                {
                    Notice.NotiSuccess("Cập nhật dữ liệu thành công");
                }
                else
                {
                    Notice.NotiSuccess("Thêm dữ liệu thành công");
                }
            }
            else
            {
                if (checkExistId.IsNotNullOrEmpty())
                {
                    Notice.NotiError("Cập nhật dữ liệu thất bại");
                }
                else
                {
                    Notice.NotiError("Thêm dữ liệu thất bại");
                }
            }
        }

        void OnClose()
        {
            thesisDefenseEdit.Close();
            visible = false;
        }

        void OnCloseForDetail()
        {
            ThesisDefenseDetail.Close();
            visibleForDetail = false;
        }

        async Task Edit(ThesisDefenseData thesisDefenseData)
        {
            ThesisDefense thesisDefense = await ThesisDefenseService.GetThesisDefenseByIdAsync(thesisDefenseData.Id);
            ShowStudentDetail(thesisDefense);
        }

        async Task DeleteThesisDefense(ThesisDefenseData thesisDefenseData)
        {
            ThesisDefense thesisDefense = await ThesisDefenseService.GetThesisDefenseByIdAsync(thesisDefenseData.Id.ToString());
            var result = await ThesisDefenseService.DeleteThesisDefenseAsync(thesisDefense);
            if (result.Equals(true))
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại");
            }
            selectedRows = null;
            StateHasChanged();
        }

        void OnRowClick(RowData<ThesisDefenseData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = thesisDefenseDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
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
                ListSelectedIds = ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        async Task DeleteAsync(ThesisDefenseData model = null)
        {
            try
            {
                var deleteModel = thesisDefenseDatas.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<ThesisDefense> thesisDefenses = new List<ThesisDefense>();
                // maping từ studentData thành student
                foreach (var thesisDefenseData in deleteModel)
                {
                    ThesisDefense thesisDefense = new ThesisDefense();
                    thesisDefense.Id = thesisDefenseData.Id;
                    thesisDefenses.Add(thesisDefense);
                }
                var result = await ThesisDefenseService.DeleteThesisDefenseListAsync(thesisDefenses);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async Task OpenDetailAsync(ThesisDefenseData data)
        {
            try
            {
                titleOfThesisDefenseDetail = data.Name;
                var StudentOfthesisDefense = new List<Student>();
                StudentOfthesisDefense = ThesisDefenseService.GetCurrentListStaff(CurrentUser.FacultyId, data.Id).Result;
                ThesisDefenseDetail.loadData(StudentOfthesisDefense, data.Id);
                visibleForDetail = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

    }
}
