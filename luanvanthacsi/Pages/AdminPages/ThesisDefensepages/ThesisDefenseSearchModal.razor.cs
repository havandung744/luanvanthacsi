using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseSearchModal : ComponentBase
    {
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [CascadingParameter] SessionData SessionData { get; set; }
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
        string currentThesisDefenseId;
        Table<StudentData>? table;
        StudentData? selectData;
        List<string> ListSelectedIds = new();
        IEnumerable<StudentData>? selectedRows;
        string facultyId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            studentDatas = new();
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
                    if (student != null)
                    {
                        listStudentThenIds.Add(student);
                    }
                }
                if (listStudentThenIds.Count == 0)
                {
                    Notice.NotiWarning("Không có học viên nào");
                    ChangeModalVisible();
                }
                else
                {
                    convert = GetViewModels(listStudentThenIds);
                    await ThesisDefenseService.UpdateStudentListByIds(convert);
                    ChangeModalVisible();
                    await ChangeStudentList.InvokeAsync();
                }
                StateHasChanged();
            }
        }

        List<Student> GetViewModels(List<StudentData> datas)
        {
            var models = new List<Student>();
            if (datas.IsNullOrEmpty())
            {
                Notice.NotiError("Không có học viên nào");
                ChangeModalVisible();
                return models;
            }
            else
            {
                models = _mapper.Map<List<Student>>(datas);
                models.ForEach(c =>
                {
                    c.ThesisDefenseId = currentThesisDefenseId;
                });
                return models;
            }
        }

        public async Task LoadAsync(string id)
        {
            currentThesisDefenseId = id;
            studentDatas.Clear();
            // lấy dữ liệu học viên chưa đăng ký đợt bảo vệ
            List<Student> students = new List<Student>();
            if (SessionData?.CurrentUser.FacultyId != null)
            {
                students = await StudentService.GetAllByIdAsync(SessionData.CurrentUser.FacultyId);
            }
            else
            {
                try
                {
                    facultyId = await localStorage.GetItemAsync<string>("facultyIdOfThesisDefense");
                }
                catch
                {
                    facultyId = null;
                }
                students = await StudentService.GetAllByIdAsync(facultyId);
            }
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
            catch (Exception)
            {
                throw;
            }

        }
    }
}
