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
        [Inject] IMapper _mapper { get; set; }
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
        List<string> ListSelectedIds = new();
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
            models = _mapper.Map<List<Student>>(datas);
            models.ForEach(c =>
            {
               c.ThesisDefenseId = currentThesisDefenseId;
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
            // lấy full dữ liệu
            //var students = await StudentService.GetAllAsync();
            // lấy dữ liệu học viên chưa đăng ký đợt bảo vệ
            var students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = students.Where(x => x.ThesisDefenseId == null).OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
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
