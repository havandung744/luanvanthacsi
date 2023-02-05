using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using System.Linq;
using AntDesign;
using luanvanthacsi.Data.Components;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Extentions;

namespace luanvanthacsi.Pages.AdminPages.ScientistPages
{
    public partial class ScientistList : ComponentBase
    {
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        List<Scientist>? scientists { get; set; }
        bool visible = false;
        ScientistEdit scientistEdit = new ScientistEdit();
        bool loading = false;
        string txtValue { get; set; }
        TaskSearchScientist taskSearchScientist = new TaskSearchScientist();
        protected override async Task OnInitializedAsync()
        {

        scientistDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            scientistDatas.Clear();
            loading = true;
            visible = false;
            StateHasChanged();
            var scientists = await ScientistService.GetAll();
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = scientists.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            scientistDatas = GetViewModels(list);
            loading = false;
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
                model.CreateDate = c.CreateDate;
                model.UpdateDate = c.UpdateDate;
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
            if (data.Id.IsNotNullOrEmpty())
            {
                Notice.NotiSuccess("Cập nhật dữ liệu thành công");
            }
            else
            {
                Notice.NotiSuccess("Thêm dữ liệu thành công");
            }
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
            Scientist scientist = await ScientistService.GetScientistByIdAsync(scientistData.Id.ToString());
            var result = await ScientistService.DeleteScientistAsync(scientist);
            if (result.Equals(true))
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại");
            }
        }

        async Task Search()
        {
            var txtSearch = taskSearchScientist.txtSearch;
            scientistDatas?.Clear();
            scientists = await ScientistService.GetListScientistBySearchAsync(txtSearch);
            scientistDatas = GetViewModels(scientists);
            StateHasChanged();
        }

        public class TaskSearchScientist
        {
            public string? txtSearch { get; set; }
        }

    }
}
