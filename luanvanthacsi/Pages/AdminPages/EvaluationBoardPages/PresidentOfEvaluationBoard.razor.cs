using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class PresidentOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale? TableLocale { get; set; }
        [Inject] NotificationService? Notice { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IUserService? UserService { get; set; }
        [Inject] ISpecializedService SpecializedService { get; set; }
        [Parameter] public int tab { get; set; }

        List<ScientistData>? scientistDatas { get; set; }
        IEnumerable<ScientistData> selectedRows { get; set; }
        Scientist? selectData;
        Table<ScientistData> table;
        [Inject] IMapper _mapper { get; set; }
        bool visible = false;
        bool loading = false;
        User CurrentUser;
        [Parameter] public string EvaluationBoardCode { get; set; }
        [Parameter] public List<string> SelectedScientistIds { get; set; }


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
            scientistDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            scientistDatas?.Clear();
            loading = true;
            StateHasChanged();
            visible = false;
            var lecturers = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = lecturers.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).Where(x => x.FacultyId == CurrentUser.FacultyId).ToList();
            var specializedList = await SpecializedService.GetAllByFacultyIdAsync(CurrentUser.FacultyId);
            foreach (var item in list)
            {
                item.SpecializedName = specializedList.Where(x => x.Id == item.SpecializedId).Select(x => x.Name).FirstOrDefault();
            }
            scientistDatas = _mapper.Map<List<ScientistData>>(list);
            int stt = 1;
            scientistDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
        }
        public async Task SetSelectedRows(List<string> ids)
        {
            try
            {
                List<Scientist> scientists = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
                var filteredObjects = scientists.Where(o => ids.Contains(o.Id));
                scientists.RemoveAll(o => !filteredObjects.Contains(o));
                List<ScientistData> scientistData = _mapper.Map<List<ScientistData>>(scientists);
                selectedRows = scientistData;
                table.SetSelection(selectedRows.Select(x => x.Id).ToArray());
                StateHasChanged();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<string> GetId()
        {
            List<string> ids = new List<string>();
            if (selectedRows != null)
            {
                foreach (var selected in selectedRows)
                {
                    ids.Add(selected.Id);
                }
            }
            return ids;
        }
    }
}
