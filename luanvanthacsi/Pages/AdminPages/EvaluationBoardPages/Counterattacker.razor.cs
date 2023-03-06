using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class Counterattacker : ComponentBase
    {

        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] ILecturersService LecturersService { get; set; }
        [Inject] IUserService UserService { get; set; }
        List<LecturersData> lecturersDatas { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<LecturersData> selectedRows;
        Lecturers? selectData;
        Table<LecturersData> table;
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
            lecturersDatas = new();
            await LoadAsync();
        }

        public async Task SetSelectedRows(List<string> ids)
        {
            //Lecturers lecturers = await LecturersService.GetAllByIdAsync();
            //LecturersData lecturersData = _mapper.Map<LecturersData>(lecturers);
            //selectedRows = new[] { lecturersData };
            //foreach (var id in ids)
            //{
            //    table.SetSelection(selectedRows.Select(x => id).ToArray());
            //}
        }

        public async Task LoadAsync()
        {
            lecturersDatas?.Clear();
            loading = true;
            visible = false;
            var lecturers = await LecturersService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = lecturers.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            lecturersDatas = _mapper.Map<List<LecturersData>>(list);
            int stt = 1;
            lecturersDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        public List<string> GetCounterattackerId()
        {
            List<string> ids = new();
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
