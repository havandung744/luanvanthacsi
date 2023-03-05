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
    public partial class President : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale? TableLocale { get; set; }
        [Inject] NotificationService? Notice { get; set; }
        [Inject] ILecturersService LecturersService { get; set; }
        [Inject] IUserService UserService { get; set; }
        List<LecturersData>? lecturersDatas { get; set; } = new();
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<LecturersData>? selectedRows; 
        Lecturers? selectData;
        Table<LecturersData>? table;
        List<string>? ListSelectedStudentIds;
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

        public string GetLecturersId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}