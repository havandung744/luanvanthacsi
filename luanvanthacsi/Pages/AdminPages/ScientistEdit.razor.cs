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

namespace luanvanthacsi.Pages.AdminPages
{
    public partial class ScientistEdit
    {
        [Parameter] public EventCallback Cancel { get; set; }
        [Parameter] public EventCallback<Scientist> ValueChange { get; set; }
        ScientistEditModel EditModel { get; set; } = new ScientistEditModel();
        Form<ScientistEditModel> form;
        public void LoadData(Scientist scientist)
        {
            EditModel.Id = scientist.Id;
            EditModel.Name = scientist.Name;
            EditModel.Code = scientist.Code;
            EditModel.Email = scientist.Email;
            EditModel.PhoneNumber = scientist.PhoneNumber;
            EditModel.AcademicRank = scientist.AcademicRank;
            EditModel.Degree = scientist.Degree;
            EditModel.CreateDate = scientist.CreateDate;
            StateHasChanged();
        }

        public void UpdateScientist()
        {
            Scientist scientist = new Scientist();
            scientist.Id = EditModel.Id;
            scientist.Name= EditModel.Name;
            scientist.Code= EditModel.Code;
            scientist.Email= EditModel.Email;
            scientist.PhoneNumber= EditModel.PhoneNumber;
            scientist.AcademicRank= EditModel.AcademicRank;
            scientist.Degree= EditModel.Degree;
            scientist.CreateDate = EditModel.CreateDate;
            ValueChange.InvokeAsync(scientist);
        }

        private void OnFinish(EditContext editContext)
        {
            UpdateScientist();
        }

        private void OnFinishFailed(EditContext editContext)
        {
            //EditModel = new ScientistEditModel();
        }

        public void Close()
        {
            EditModel = new ScientistEditModel();
            StateHasChanged();
        }
        private void Reset(MouseEventArgs args)
        {
            form.Reset();
        }


    }
}
