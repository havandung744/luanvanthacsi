using AutoMapper;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseDetail : ComponentBase
    {
        [Inject] IThesisDefenseService thesisDefenseService { get; set; }
        List<StudentOfThesisDefenseEditModel> studentOfThesisDefenseEditModels { get; set; }
        protected override async Task OnInitializedAsync()
        {
            studentOfThesisDefenseEditModels = new();
        }

        public async Task loadData(List<Student> students)
        {
            foreach (var student in students)
            {
                StudentOfThesisDefenseEditModel studentOfThesisDefenseEditModel = new StudentOfThesisDefenseEditModel();
                studentOfThesisDefenseEditModel.Id = student.Id;
                studentOfThesisDefenseEditModel.Code = student.Code;
                studentOfThesisDefenseEditModel.Name = student.Name;
                studentOfThesisDefenseEditModel.Email = student.Email;
                studentOfThesisDefenseEditModel.PhoneNumber = student.PhoneNumber;
                studentOfThesisDefenseEditModel.DateOfBirth = student.DateOfBirth.ToString();
                studentOfThesisDefenseEditModels.Add(studentOfThesisDefenseEditModel);
            }
            StateHasChanged();
        }
        public void Close()
        {
            studentOfThesisDefenseEditModels = new List<StudentOfThesisDefenseEditModel>();
            StateHasChanged();
        }

    }
}
