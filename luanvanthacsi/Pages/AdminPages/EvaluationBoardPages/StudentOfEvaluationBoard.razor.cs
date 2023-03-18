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
using luanvanthacsi.Data.Edit;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class StudentOfEvaluationBoard : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        List<StudentData>? studentDatas { get; set; }
        IEnumerable<StudentData> selectedRows;
        StudentData? selectData;
        Table<StudentData> table;
        public EvaluationBoardEditModel evaluationBoardEditModel { get; set; } = new EvaluationBoardEditModel();
        [Inject] IMapper _mapper { get; set; }
        bool visible = false;
        bool loading = false;
        User CurrentUser;
        [Parameter] public string EvaluationBoardCode { get; set; }
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

        public async Task SetSelectedRows(string id,string currentId)
        {
            // lấy ra thông tin base gồm mã và tên hội đồng đánh giá
            EvaluationBoard evaluationBoard = await EvaluationBoardService.GetEvaluationBoardByIdAsync(currentId);
            evaluationBoardEditModel.Code= evaluationBoard.Code;
            evaluationBoardEditModel.Name= evaluationBoard.Name;

            // lấy thông học viên bảo vệ
            Student student = await StudentService.GetStudentByIdAsync(id);
            StudentData studentData = _mapper.Map<StudentData>(student);
            selectedRows = new[] { studentData };
            table.SetSelection(selectedRows.Select(x => id).ToArray());
        }

        async Task<string> AddEvaluationBoard()
        {

            var evaluationBoards = await EvaluationBoardService.GetAllByIdAsync(CurrentUser.FacultyId);
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = evaluationBoards.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            var evaluationBoardDatas = _mapper.Map<List<EvaluationBoardData>>(list);

            var lastCode = evaluationBoardDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 1;
            if (lastCode != null && int.TryParse(lastCode.Substring(4), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "HDDG" + codeNumber.ToString("D3");
            return newCode;
        }

        public async Task LoadAsync()
        {
            evaluationBoardEditModel.Code = await AddEvaluationBoard();
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

        public EvaluationBoardEditModel GetInfoBase()
        {
            return evaluationBoardEditModel; 
        }

        public string GetStudentId()
        {
            return selectedRows?.FirstOrDefault()?.Id;
        }
    }
}
