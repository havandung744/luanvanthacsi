using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseDetail : ComponentBase
    {
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IMapper _mapper { get; set; }
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
            foreach (var student in students)
            {
                // lấy id của đợt bảo vệ hiện đang truy cập.
                StudentData studentData = new StudentData();
                studentData.Id = student.Id;
                studentData.Code = student.Code;
                studentData.Name = student.Name;
                studentData.Email = student.Email;
                studentData.PhoneNumber = student.PhoneNumber;
                studentData.DateOfBirth = student.DateOfBirth;
                studentData.CreateDate = student.CreateDate;
                studentDatas?.Add(studentData);
            }
            StateHasChanged();
        }
        public void Close()
        {
            studentDatas = new List<StudentData>();
            StateHasChanged();
        }

        public async Task LoadAsync()
        {
            studentDatas.Clear();
            //loading = true;
            //visible = false;
            //visibleForDetail = false;
            var students = await ThesisDefenseService.GetCurrentListStaff(CurrenUser.FacultyId, currentThesisDefense);
            //var thesisDefenses = await ThesisDefenseService.GetAllAsync();
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = students.OrderByDescending(x => x.CreateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
            //loading = false;
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
            studentUp.FacultyId = CurrenUser.FacultyId;
            studentUp.ThesisDefenseId = null;
            await ThesisDefenseService.UpdateStudentById(studentUp);
            LoadAsync();
            StateHasChanged();
        }

        void ChangeModalVisible()
        {
            modalVisible = false;
        }


    }
}
