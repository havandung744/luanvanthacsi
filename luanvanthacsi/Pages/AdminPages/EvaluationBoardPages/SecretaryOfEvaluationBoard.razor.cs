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
    public partial class SecretaryOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale? TableLocale { get; set; }
        [Inject] NotificationService? Notice { get; set; }
        [Inject] ISecretaryService? SecretaryService { get; set; }
        [Inject] IUserService? UserService { get; set; }
        List<SecretaryData>? secretaryDatas { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<SecretaryData>? selectedRows;
        Lecturers? selectData;
        Table<SecretaryData>? table;
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
            secretaryDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            secretaryDatas?.Clear();
            loading = true;
            visible = false;
            var lecturers = await SecretaryService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = lecturers.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            secretaryDatas = _mapper.Map<List<SecretaryData>>(list);
            int stt = 1;
            secretaryDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        public string GetSecretaryId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}
