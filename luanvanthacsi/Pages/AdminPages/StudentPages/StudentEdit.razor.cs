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

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentEdit
    {
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Student> ValueChange { get; set; }
        [Parameter] public User CurrentUser { get; set; }
        StudentEditModel EditModel { get; set; } = new StudentEditModel();
        Form<StudentEditModel> form;
        public void LoadData(Student student)
        {
            EditModel.Id = student.Id;
            EditModel.Name = student.Name;
            EditModel.Code = student.Code;
            EditModel.Email = student.Email;
            EditModel.PhoneNumber = student.PhoneNumber;
            EditModel.UpdateDate = student.UpdateDate;
            EditModel.CreateDate = student.CreateDate;
            //EditModel.FacultyId = student.FacultyId;
            if(student.DateOfBirth==DateTime.MinValue)
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

        public void UpdateStudent()
        {
            Student student = new Student();
            student.Id = EditModel.Id;
            student.Name = EditModel.Name;
            student.Code = EditModel.Code;
            student.Email = EditModel.Email;
            student.PhoneNumber = EditModel.PhoneNumber;
            student.CreateDate = EditModel.CreateDate;
            student.UpdateDate = EditModel.UpdateDate;
            student.DateOfBirth = EditModel.DateOfBirth;
            student.FacultyId = CurrentUser.FacultyId;
            //student.ThesisDefenseId = EditModel
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
        private void Reset(MouseEventArgs args)
        {
            form.Reset();
        }


    }
}
