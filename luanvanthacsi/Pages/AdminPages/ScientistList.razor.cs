using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using System.Linq;

namespace luanvanthacsi.Pages.AdminPages
{
    public partial class ScientistList
    {
        [Inject] IScientistService ScientistService { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        bool visible = false;
        ScientistEdit scientistEdit = new ScientistEdit();
        bool loading;
        protected override async Task OnInitializedAsync()
        {
            scientistDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            scientistDatas.Clear();
            var scientists = await ScientistService.GetAll();
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = scientists.OrderByDescending(x => x.CreateDate).ToList();
            scientistDatas = GetViewModels(list);
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
                model.Id = c.Id;
                model.stt = stt;
                model.Name = c.Name;
                model.Email = c.Email;
                model.Code = c.Code;
                model.PhoneNumber = c.PhoneNumber;       
                model.AcademicRank = c.AcademicRank;       
                model.Degree = c.Degree;
                model.CreateDate= c.CreateDate;
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
            loading = false;
        }

        void OnClose()
        {
            scientistEdit.Close();
            visible = false;
        }

        async Task Edit(ScientistData scientistData)
        {
            Scientist scientis = await ScientistService.GetScientistByIdAsync(scientistData.Id);
            ShowScientistDetail(scientis);
        }

        async Task DeleteScientist(ScientistData scientistData)
        {
            loading= true;
            Scientist scientist = await ScientistService.GetScientistByIdAsync(scientistData.Id.ToString());
            await ScientistService.DeleteScientistAsync(scientist);
            LoadAsync();
            loading = false;
        }


    }
}
