using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class StudentOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }

        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Parameter] public string EvaluationBoardCode { get; set; }
        List<StudentData>? studentDatas { get; set; }
        IEnumerable<StudentData> selectedRows;
        StudentData? selectData;
        Table<StudentData> table;
        public EvaluationBoardEditModel evaluationBoardEditModel { get; set; } = new EvaluationBoardEditModel();
        [Inject] IMapper _mapper { get; set; }
        bool visible = false;
        bool loading = false;
        User CurrentUser;

        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            studentDatas = new();
            await LoadAsync();
        }

        public async Task SetSelectedRows(string id, string currentId)
        {
            // lấy thông học viên bảo vệ
            selectedRows = null;
            Student student = await StudentService.GetStudentByIdAsync(id);
            StudentData studentData = _mapper.Map<StudentData>(student);
            selectedRows = new[] { studentData };
            table.SetSelection(selectedRows.Select(x => id).ToArray());
        }

        public async Task LoadAsync()
        {
            studentDatas?.Clear();
            loading = true;
            StateHasChanged();
            visible = false;
            List<Student> students = new List<Student>();
            if (CurrentUser.FacultyId == null)
            {
                var facultyId = await localStorage.GetItemAsync<string>("facultyIdOfEvaluation");
                students = await StudentService.GetAllByIdAsync(facultyId);
            }
            else
            {
                students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            }
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
            int stt = 1;
            studentDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        public string GetStudentId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}
