using AntDesign.TableModels;
using AntDesign;
using BlazorInputFile;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using Microsoft.AspNetCore.Components;
using AutoMapper;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Ultils;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class StudentOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IUserService UserService { get; set; }
        List<StudentData>? studentDatas { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<StudentData> selectedRows;
        StudentData? selectData;
        Table<StudentData> table;
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
            studentDatas = new();
            await LoadAsync();
        }

        public async Task SetSelectedRows(string id)
        {
            Student student = await StudentService.GetStudentByIdAsync(id);
            StudentData studentData = _mapper.Map<StudentData>(student);
            selectedRows = new[] { studentData };
            table.SetSelection(selectedRows.Select(x => id).ToArray());
        }

        public async Task LoadAsync()
        {
            studentDatas?.Clear();
            loading = true;
            visible = false;
            var students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
            int stt = 1;
            studentDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        public string GetStudentId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}
