using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;

namespace luanvanthacsi.Pages.AdminPages
{
    public partial class ScientistList
    {
        [Inject] IScientistService ScientistService { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        bool visible = false;
        ScientistEdit scientistEdit = new ScientistEdit();
        bool loading = false;
        protected override async Task OnInitializedAsync()
        {
            loading = true;
            scientistDatas = new();
            await LoadAsync();
            var cientists = ScientistService.GetAll();
            loading= false;
        }

        public async Task LoadAsync()
        {
            scientistDatas.Clear();
            var scientists = await ScientistService.GetAll();
            scientistDatas = GetViewModels(scientists);
            StateHasChanged();
        }

        List<ScientistData> GetViewModels(List<Scientist> datas)
        {
            var models = new List<ScientistData>();
            ScientistData model;
            int stt = 1;
            datas.ForEach(c =>
            {
                model = new ScientistData();
                model.stt = stt;
                model.Name = c.Name;
                model.Email = c.Email;
                model.Code = c.Code;
                model.PhoneNumber = c.PhoneNumber;

                
                models.Add(model);
                stt++;
            });
            return models;
        }

        void AddScientist()
        {
            var scientistData = new Scientist();
            ShowScientistDetail(scientistData);
        }

        void ShowScientistDetail(Scientist data)
        {
            scientistEdit.LoadData(data);
            visible = true;
        }

        async Task Save(Scientist data)
        {
            var resultAdd = await ScientistService.AddOrUpdateScientist(data);
            await LoadAsync();
            visible= false;
        }

    }
}
