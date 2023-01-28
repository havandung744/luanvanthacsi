using luanvanthacsi.Data.Edit;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;
using luanvanthacsi.Data;

namespace luanvanthacsi.Pages.AdminPages
{
    public partial class ScientistEdit
    {
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Scientist> ValueChange { get; set; }
        ScientistEditModel EditModel { get; set; } = new ScientistEditModel();
        public void LoadData(Scientist scientist)
        {
            EditModel.Id = scientist.Id;
            EditModel.Name = scientist.Name;
            EditModel.Code = scientist.Code;
            EditModel.Email = scientist.Email;
            EditModel.PhoneNumber = scientist.PhoneNumber;
            StateHasChanged();
        }

    }
}
