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
        TaskSearch taskSearch = new TaskSearch();
        IEnumerable<ThesisDefenseData>? selectRows;
        ThesisDefenseData selectData;
        Table<ThesisDefenseData>? table;
        List<string>? ListSelectedThesisDefenseIds;
        IEnumerable<ThesisDefenseData>? selectedRows;
        List<string>? ListSelectedIds;
        User Currentuser;
        bool visible = false;
        bool visibleForDetail = false;
        bool loading = false;
        string txtValue { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // lấy thông tin User đang đăng nhập
            string id = await getUserId();
            Currentuser = await UserService.GetUserByIdAsync(id);

            thesisDefenseDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            thesisDefenseDatas.Clear();
            loading = true;
            visible = false;
            visibleForDetail = false;
            StateHasChanged();
            var thesisDefenses = await ThesisDefenseService.GetAllAsync();
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = thesisDefenses.OrderByDescending(x => x.CreateDate).ToList();
            thesisDefenseDatas = GetViewModels(list);
            loading = false;
            StateHasChanged();
        }

        List<ThesisDefenseData> GetViewModels(List<ThesisDefense> datas)
        {
            var models = new List<ThesisDefenseData>();
            ThesisDefenseData model;
            int stt = 1;
            datas.ForEach(c =>
            {
                model = new ThesisDefenseData();
                model.Id = c.Id;
                model.stt = stt;
                model.Name = c.Name;
                model.Code = c.Code;
                model.CreateDate = c.CreateDate;
                models.Add(model);
                stt++;
            });
            return models;
        }

        void AddThesisDefense()
        {
            var thesisDefenseData = new ThesisDefense();
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

        async Task DeleteStudent(ThesisDefenseData thesisDefenseData)
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

        async Task Search()
        {
            var txtSearch = taskSearch.txtSearch;
            thesisDefenseDatas?.Clear();
            thesisDefenses = await ThesisDefenseService.GetListThesisDefenseBySearchAsync(txtSearch);
            thesisDefenseDatas = GetViewModels(thesisDefenses);
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

        //void OnChangeIsShow(bool isShow)
        //{
        //    showExcelForm = isShow;
        //}

        public class TaskSearch
        {
            public string? txtSearch { get; set; }
        }
        async Task OpenDetailAsync(ThesisDefenseData data)
        {
            try
            {
                var StudentOfthesisDefense = new List<Student>();
                StudentOfthesisDefense = ThesisDefenseService.GetCurrentListStaff(Currentuser.FacultyId, data.Id).Result;
                await ThesisDefenseDetail.loadData(StudentOfthesisDefense);
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
