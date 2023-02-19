using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Edit;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseEdit : ComponentBase
    {
        [Parameter] public User CurrentUser { get; set; }
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<ThesisDefense> ValueChange { get; set; }
        ThesisDefenseEditModel EditModel { get; set; } = new ThesisDefenseEditModel();
        Form<ThesisDefenseEditModel> form;
        public void LoadData(ThesisDefense thesisDefense)
        {
            EditModel.Id = thesisDefense.Id;
            EditModel.Name = thesisDefense.Name;
            EditModel.Code = thesisDefense.Code;
            EditModel.CreateDate = thesisDefense.CreateDate;
            EditModel.YearOfProtection = thesisDefense.YearOfProtection;
            StateHasChanged();
        }

        public void UpdateThesisDefense()
        {
            ThesisDefense thesisDefense = new ThesisDefense();
            thesisDefense.Id = EditModel.Id;
            thesisDefense.Name = EditModel.Name;
            thesisDefense.Code = EditModel.Code;
            thesisDefense.CreateDate = EditModel.CreateDate;
            thesisDefense.YearOfProtection = EditModel.YearOfProtection;
            thesisDefense.FacultyId = CurrentUser.FacultyId;
            ValueChange.InvokeAsync(thesisDefense);
        }

        private void OnFinish(EditContext editContext)
        {
            UpdateThesisDefense();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //EditModel = new ScientistEditModel();
        }

        public void Close()
        {
            EditModel = new ThesisDefenseEditModel();
            StateHasChanged();
        }
        private void Reset(MouseEventArgs args)
        {
            form.Reset();
        }

    }
}
