using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseDetail : ComponentBase
    {
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Parameter] public string facultyId { get; set; }
        List<StudentData>? studentDatas { get; set; }
        ThesisDefenseSearchModal? thesisDefenseSearchModal;
        bool modalVisible;
        string currentThesisDefense;
        User CurrenUser;

        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrenUser = await UserService.GetUserByIdAsync(id);
            studentDatas = new();
        }

        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

        public void loadData(List<Student> students, string id)
        {
            currentThesisDefense = id;
            studentDatas = _mapper.Map<List<StudentData>>(students);
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
            if (CurrenUser.FacultyId == null)
            {
                students = await ThesisDefenseService.GetCurrentListStaff(facultyId, currentThesisDefense);
            }
            else
            {
                students = await ThesisDefenseService.GetCurrentListStaff(CurrenUser.FacultyId, currentThesisDefense);
            }
            var list = students.OrderByDescending(x => x.CreateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
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
            if (CurrenUser.FacultyId == null)
            {
                studentUp.FacultyId = facultyId;
            }
            else
            {
                studentUp.FacultyId = CurrenUser.FacultyId;
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
