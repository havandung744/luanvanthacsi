using AntDesign;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Services;
using AutoMapper;

namespace luanvanthacsi.Pages.AdminPages.ThesisDefensepages
{
    public partial class ThesisDefenseEdit : ComponentBase
    {
        [Inject] IMapper _mapper { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
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
            thesisDefense = _mapper.Map<ThesisDefense>(EditModel);
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
        private async Task Reset(MouseEventArgs args)
        {
            if (EditModel.Id == null)
            {
                var newCode = EditModel.Code;
                EditModel = new ThesisDefenseEditModel();
                EditModel.Code = newCode;
            }
            else
            {
                ThesisDefense oldEditmodel = await ThesisDefenseService.GetThesisDefenseByIdAsync(EditModel.Id);
                EditModel = _mapper.Map<ThesisDefenseEditModel>(oldEditmodel);
            }
            StateHasChanged();
        }

    }
}
