﻿using AntDesign;
using AutoMapper;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class ScientistOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale? TableLocale { get; set; }
        [Inject] NotificationService? Notice { get; set; }
        [Inject] IScientistService? ScientistService { get; set; }
        [Inject] IUserService? UserService { get; set; }
        List<ScientistData>? scientistDatas { get; set; }
        IEnumerable<ScientistData>? selectedRows;
        Scientist? selectData;
        Table<ScientistData>? table;
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
            scientistDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            scientistDatas?.Clear();
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

        public List<string> GetScientistsId()
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