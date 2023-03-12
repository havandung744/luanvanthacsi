using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data;
using System.Net.NetworkInformation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using AntDesign;
using Microsoft.AspNetCore.Components.Web;
using AutoMapper;
using luanvanthacsi.Data.Services;

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentEdit
    {
        [Inject] IMapper _mapper { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Student> ValueChange { get; set; }
        [Parameter] public User CurrentUser { get; set; }
        StudentEditModel EditModel { get; set; } = new StudentEditModel();
        Form<StudentEditModel> form;
        public void LoadData(Student student)
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
            StateHasChanged();
        }
        private bool DisabledDate(DateTime current)
        {
            return current > DateTime.Now;
        }

        public void UpdateStudent()
        {
            Student student = new Student();
            student = _mapper.Map<Student>(EditModel);
            student.FacultyId = CurrentUser.FacultyId;
            ValueChange.InvokeAsync(student);
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
