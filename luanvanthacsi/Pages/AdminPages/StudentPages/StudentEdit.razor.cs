using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentEdit
    {
        [Inject] IMapper _mapper { get; set; }
        [Inject] AntDesign.NotificationService Notice { get; set; }
        [Inject] ISpecializedService SpecializedService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Student> ValueChange { get; set; }
        [Parameter] public User CurrentUser { get; set; }
        [Parameter] public string facultyId { get; set; }
        StudentEditModel EditModel { get; set; } = new StudentEditModel();
        Form<StudentEditModel> form;
        List<Scientist> scientistList { get; set; }
        List<Specialized> specializedList { get; set; }


        public async Task LoadData(Student student)
        {
            EditModel = _mapper.Map<StudentEditModel>(student);
            if (student.DateOfBirth == DateTime.MinValue)
            {
                EditModel.DateOfBirth = DateTime.Now;
            }
            else
            {
                EditModel.DateOfBirth = student.DateOfBirth;
            }
            EditModel.UpdateDate = student.UpdateDate;
            if (CurrentUser.FacultyId == null)
            {
                scientistList = await ScientistService.GetAllByIdAsync(facultyId);
                specializedList = await SpecializedService.GetAllByFacultyIdAsync(facultyId);
            }
            else
            {
                scientistList = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
                specializedList = await SpecializedService.GetAllByFacultyIdAsync(CurrentUser.FacultyId);
            }
            StateHasChanged();
        }

        private bool DisabledDate(DateTime current)
        {
            return current > DateTime.Now;
        }

        public void UpdateStudent()
        {
            try
            {
                if (EditModel.InstructorIdOne == EditModel.InstructorIdTwo)
                {
                    Notice.NotiWarning("Hướng dẫn 1 và hướng dẫn 2 bị trùng lặp");
                    return;
                }
                Student student = new Student();
                student = _mapper.Map<Student>(EditModel);
                if (CurrentUser.FacultyId == null)
                {
                    student.FacultyId = facultyId;
                }
                else
                {
                    student.FacultyId = CurrentUser.FacultyId;
                }
                ValueChange.InvokeAsync(student);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void OnFinish(EditContext editContext)
        {
            UpdateStudent();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //EditModel = new ScientistEditModel();
        }

        public void Close()
        {
            EditModel = new StudentEditModel();
            StateHasChanged();
        }
        private async Task Reset(MouseEventArgs args)
        {
            if (EditModel.Id == null)
            {
                var newCode = EditModel.Code;
                EditModel = new StudentEditModel();
                EditModel.Code = newCode;
            }
            else
            {
                Student oldEditmodel = await StudentService.GetStudentByIdAsync(EditModel.Id);
                EditModel = _mapper.Map<StudentEditModel>(oldEditmodel);
            }
            StateHasChanged();
        }


    }
}
