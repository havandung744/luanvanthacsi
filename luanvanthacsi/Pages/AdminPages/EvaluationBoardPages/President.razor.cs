using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class President : ComponentBase
    {
        [Parameter] public string PresidentSelect { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale? TableLocale { get; set; }
        [Inject] NotificationService? Notice { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IUserService UserService { get; set; }
        List<ScientistData> scientistDatas { get; set; } = new();
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<ScientistData> selectedRows; 
        Table<ScientistData> table;
        [Inject] IMapper _mapper { get; set; }
        bool visible = false;
        bool loading = false;
        User CurrentUser;
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            await LoadAsync();
        }

        public async Task SetSelectedRows(string id)
        {
            Scientist scientist = await ScientistService.GetScientistByIdAsync(id);
            ScientistData scientistData = new ScientistData();
            scientistData.Id = id;
            selectedRows = new[] { scientistData };
            table.SetSelection(selectedRows.Select(x => id).ToArray());
        }


        public async Task LoadAsync()
        {
            scientistDatas.Clear();
            loading = true;
            visible = false;
            var lecturers = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = lecturers.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            scientistDatas = _mapper.Map<List<ScientistData>>(list);
            int stt = 1;
            scientistDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        public string GetLecturersId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}
