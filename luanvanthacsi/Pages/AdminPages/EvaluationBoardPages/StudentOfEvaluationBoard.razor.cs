using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class StudentOfEvaluationBoard : ComponentBase
    {
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [CascadingParameter] public SessionData SessionData { get; set; }
        [Parameter] public string EvaluationBoardCode { get; set; }
        [Parameter] public bool CheckIsAdd { get; set; }
        List<StudentData>? studentDatas { get; set; }
        IEnumerable<StudentData> selectedRows;
        StudentData? selectData;
        Table<StudentData> table;
        public EvaluationBoardEditModel evaluationBoardEditModel { get; set; } = new EvaluationBoardEditModel();
        List<Scientist> scientists { get; set; }
        bool visible = false;
        bool loading = false;

        protected override async Task OnInitializedAsync()
        {
            studentDatas = new();
            scientists = await ScientistService.GetAll();
            //await LoadAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAsync();
            }
        }

        public async Task SetSelectedRows(string id, string currentId)
        {
            // lấy thông học viên bảo vệ
            Student student = await StudentService.GetStudentByIdAsync(id);
            StudentData studentData = _mapper.Map<StudentData>(student);
            selectedRows = new[] { studentData };
            table.SetSelection(selectedRows.Select(x => id).ToArray());
        }

        public async Task LoadAsync()
        {
            loading = true;
            StateHasChanged();
            visible = false;
            studentDatas?.Clear();
            List<Student> students = new List<Student>();
            if (SessionData.CurrentUser?.FacultyId == null)
            {
                string facultyId = await localStorage.GetItemAsync<string>("facultyIdOfEvaluation");
                students = await StudentService.GetAllByIdAsync(facultyId);
            }
            else
            {
                students = await StudentService.GetAllByIdAsync(SessionData.CurrentUser.FacultyId);
            }
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            foreach (var item in list)
            {
                item.InstructorNameOne = scientists.Where(x => x.Id == item?.InstructorIdOne).Select(x => x.Name).FirstOrDefault();
                item.InstructorNameTwo = scientists.Where(x => x.Id == item?.InstructorIdTwo).Select(x => x.Name).FirstOrDefault();
            }
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
        public List<string> GetInstructorIdsOfStudent()
        {
            List<string> ids = new List<string>();
            ids.Add(selectedRows?.FirstOrDefault()?.InstructorIdOne);
            ids.Add(selectedRows?.FirstOrDefault()?.InstructorIdTwo);
            return ids;
        }
    }
}
