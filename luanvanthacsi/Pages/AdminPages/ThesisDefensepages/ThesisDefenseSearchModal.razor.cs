using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseSearchModal : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Parameter] public bool ModalVisible { get; set; }
        [Parameter] public EventCallback<bool> ChanageModalVisible { get; set; }
        [Parameter] public EventCallback ReloadStudentList { get; set; }
        [Parameter] public bool MultipleSelect { get; set; }
        [Parameter] public EventCallback<ThesisDefenseData> ValueChanged { get; set; }
        [Parameter] public EventCallback<List<ThesisDefenseData>> ListSelectedChanged { get; set; }
        private IEnumerable<StudentData> _selectedRows;
        [Parameter] public EventCallback CancelChanged { get; set; }
        [Parameter] public EventCallback ChangeStudentList { get; set; }
        List<StudentData> studentDatas { get; set; } = new List<StudentData>();
        User CurrentUser;
        string currentThesisDefenseId;
        Table<StudentData>? table;
        StudentData? selectData;
        List<string> ListSelectedIds;
        IEnumerable<StudentData>? selectedRows;
        List<StudentData> studentOfThesisDefenseEditModels { get; set; }
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            studentDatas = new();
            //selectedTripId = new();
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }
        async Task HandleOk()
        {
            if (ListSelectedIds.Count > 0)
            {
                List<Student> convert = new List<Student>();
                List<StudentData> listStudentThenIds = new List<StudentData>();
                foreach (string item in ListSelectedIds)
                {
                    var student = studentDatas.Where(x => x.Id == item).FirstOrDefault();
                    listStudentThenIds.Add(student);
                }
                    convert = GetViewModels(listStudentThenIds);
                    await ThesisDefenseService.UpdateStudentListByIds(convert);
                    ChangeModalVisible();
                    await ChangeStudentList.InvokeAsync();
                    StateHasChanged();
            }
        }

        List<Student> GetViewModels(List<StudentData> datas)
        {
            var models = new List<Student>();
            Student model;
            datas.ForEach(c =>
            {
                model = new Student();
                model.Id = c.Id;
                model.Name = c.Name;
                model.Email = c.Email;
                model.Code = c.Code;
                model.PhoneNumber = c.PhoneNumber;
                model.DateOfBirth = c.DateOfBirth;
                model.CreateDate = c.CreateDate;
                model.UpdateDate = c.UpdateDate;
                model.FacultyId = c.FacultyId;
                model.ThesisDefenseId = currentThesisDefenseId;
                models.Add(model);
            });
            return models;
        }

        /// <summary>
        /// thực hiện gọi db lấy dữ liệu để hiển thị
        /// Author: hvdung
        /// Create:19/02/2023
        /// </summary>
        public async Task LoadAsync(string id)
        {
            currentThesisDefenseId = id;
            studentDatas.Clear();
            //loading = true;
            //visible = false;
            StateHasChanged();
            // lấy full dữ liệu
            //var students = await StudentService.GetAllAsync();
            // lấy dữ liệu học viên chưa đăng ký đợt bảo vệ
            var students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = students.Where(x => x.ThesisDefenseId == null).OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = GetViewModels(list);
            //loading = false;
            StateHasChanged();
        }

        List<StudentData> GetViewModels(List<Student> datas)
        {
            var models = new List<StudentData>();
            StudentData model;
            int stt = 1;
            datas.ForEach(c =>
            {
                model = new StudentData();
                model.Id = c.Id;
                model.stt = stt;
                model.Name = c.Name;
                model.Email = c.Email;
                model.Code = c.Code;
                model.PhoneNumber = c.PhoneNumber;
                model.DateOfBirth = c.DateOfBirth;
                model.CreateDate = c.CreateDate;
                model.UpdateDate = c.UpdateDate;
                model.FacultyId = c.FacultyId;
                models.Add(model);
                stt++;
            });
            return models;
        }

        void ChangeModalVisible()
        {
            ChanageModalVisible.InvokeAsync();
        }

        void OnRowClick(RowData<StudentData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = studentDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
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
    }
}
