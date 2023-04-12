using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseDetail : ComponentBase
    {
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [CascadingParameter] SessionData SessionData { get; set; }
        [Parameter] public string facultyId { get; set; }
        List<StudentData>? studentDatas { get; set; }
        ThesisDefenseSearchModal? thesisDefenseSearchModal;
        bool modalVisible;
        string currentThesisDefense;

        protected override async Task OnInitializedAsync()
        {
            studentDatas = new();
        }

        public void loadData(List<Student> students, string id)
        {
            currentThesisDefense = id;
            studentDatas = _mapper.Map<List<StudentData>>(students);
            int stt = 1;
            studentDatas.ForEach(x => { x.stt = stt++; });
            StateHasChanged();
        }
        public void Close()
        {
            studentDatas = new List<StudentData>();
            StateHasChanged();
        }

        public async Task LoadAsync()
        {
            studentDatas?.Clear();
            List<Student> students = new List<Student>();
            if (SessionData.CurrentUser?.FacultyId == null)
            {
                students = await ThesisDefenseService.GetCurrentListStaff(facultyId, currentThesisDefense);
            }
            else
            {
                students = await ThesisDefenseService.GetCurrentListStaff(SessionData.CurrentUser.FacultyId, currentThesisDefense);
            }
            var list = students.OrderByDescending(x => x.CreateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
            int stt = 1;
            studentDatas.ForEach(x => { x.stt = stt++; });
            StateHasChanged();
        }


        async Task AddListThesisDefenseAsync()
        {
            await thesisDefenseSearchModal.LoadAsync(currentThesisDefense);
            modalVisible = true;
        }

        async Task RemoveThesisDefense(StudentData student)
        {
            var studentUp = _mapper.Map<Student>(student);
            if (SessionData.CurrentUser?.FacultyId == null)
            {
                studentUp.FacultyId = facultyId;
            }
            else
            {
                studentUp.FacultyId = SessionData.CurrentUser.FacultyId;
            }
            studentUp.ThesisDefenseId = null;
            await ThesisDefenseService.UpdateStudentById(studentUp);
            await LoadAsync();
            StateHasChanged();
        }

        void ChangeModalVisible()
        {
            modalVisible = false;
        }


    }
}
