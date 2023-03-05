using AntDesign;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardAddLayout : ComponentBase
    {
        [Parameter] public User CurrentUser { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] NotificationService Notice { get; set; }
        public StudentOfEvaluationBoard StudentOfEvaluationBoardRef { get; set; } = new();
        public President PresidentRef { get; set; } = new();
        Counterattacker CounterattackerRef { get; set; } = new();
        ScientistOfEvaluationBoard ScientistOfEvaluationBoardRef { get; set; } = new();
        SecretaryOfEvaluationBoard SecretaryOfEvaluationBoardRef { get; set; } = new();
        List<string> keys = new List<string>();
        [Parameter] public EventCallback<EvaluationBoard> SaveChange { get; set; }


        protected override async Task OnInitializedAsync()
        {
        }

        void OnTabChange(string key)
        {
        }

        async Task SaveAsync()
        {
            EvaluationBoard evaluationBoard = new EvaluationBoard();
            // lấy id của học viên bảo vệ
            var userId = StudentOfEvaluationBoardRef.GetStudentId();
            if (userId == null)
            {
                Notice.NotiError("Vui lòng chọn học viên!");
                return;
            }
            else
            {
                evaluationBoard.StudentId = userId;
            }
            // lấy id của chủ tịch hội đồng bảo vệ
            var presidentId = PresidentRef.GetLecturersId();
            if (presidentId == null)
            {
                Notice.NotiError("Vui lòng chọn chủ tịch hội đồng!");
                return;
            }
            else
            {
                evaluationBoard.PresidentId = presidentId;
            }
            // lấy danh sách id của giảng viên phản biện
            var counterattackerIds = CounterattackerRef.GetCounterattackerId();
            if (counterattackerIds.Count != 3)
            {
                Notice.NotiError("Vui lòng chọn ba phản biện!");
                return;
            }
            else
            {
                evaluationBoard.CounterattackerIdOne = counterattackerIds[0];
                evaluationBoard.CounterattackerIdTwo = counterattackerIds[1];
                evaluationBoard.CounterattackerIdThree = counterattackerIds[2];
            }
            // lấy id của 2 nhà khoa học
            var scientistIds = ScientistOfEvaluationBoardRef.GetScientistsId();
            if (scientistIds.Count != 2)
            {
                Notice.NotiError("Vui lòng chọn hai nhà khoa học!");
                return;
            }
            else
            {
                evaluationBoard.ScientistIdOne = scientistIds[0];
                evaluationBoard.ScientistIdTwo = scientistIds[1];
            }
            // lấy id của thư ký
            var secretaryId = SecretaryOfEvaluationBoardRef.GetSecretaryId();
            if (secretaryId == null)
            {
                Notice.NotiError("Vui lòng chọn thư ký!");
                return;
            }
            else
            {
                evaluationBoard.SecretaryId = secretaryId;
            }
            // Thực hiện lưu
            evaluationBoard.FacultyId = CurrentUser.Id;
            await SaveChange.InvokeAsync(evaluationBoard);

        }

        public async Task LoadDetail(EvaluationBoard data)
        {
           await StudentOfEvaluationBoardRef.SetSelectedRows(data.StudentId);
        }

        void CancelAsync()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        }

    }
}
