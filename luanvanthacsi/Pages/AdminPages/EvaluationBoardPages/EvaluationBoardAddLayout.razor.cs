using AntDesign;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardAddLayout : ComponentBase
    {
        [Parameter] public User CurrentUser { get; set; }
        [Inject] NotificationService Notice { get; set; }
        public StudentOfEvaluationBoard StudentOfEvaluationBoardRef { get; set; } = new();
        public President PresidentRef { get; set; } = new();
        Counterattacker CounterattackerRef { get; set; } = new();
        ScientistOfEvaluationBoard ScientistOfEvaluationBoardRef { get; set; } = new();
        SecretaryOfEvaluationBoard SecretaryOfEvaluationBoardRef { get; set; } = new();
        [Parameter] public EventCallback<EvaluationBoard> SaveChange { get; set; }
        [Parameter] public EventCallback CancelDetail { get; set; }
        private string activeTab = "1";
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        async Task OnTabChange(string key)
        {
            activeTab = key;
        }

        async Task SaveAsync()
        {
            try
            {
                EvaluationBoard evaluationBoard = new EvaluationBoard();
                var infoBase = StudentOfEvaluationBoardRef.GetInfoBase();
                // lấy mã hội đồng bảo vệ
                var code = infoBase.Code;
                evaluationBoard.Code = code;
                // lấy tên hội đồng bảo vệ
                var name = infoBase.Name;
                evaluationBoard.Name = name;
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
                activeTab = "1";
                evaluationBoard.FacultyId = CurrentUser.FacultyId;
                await SaveChange.InvokeAsync(evaluationBoard);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task LoadDetail(EvaluationBoard data)
        {
            try
            {
                // lấy danh sách id phản biện
                List<string> CounterattackerIds = new List<string>();
                CounterattackerIds.Add(data.CounterattackerIdOne);
                CounterattackerIds.Add(data.CounterattackerIdTwo);
                CounterattackerIds.Add(data.CounterattackerIdThree);

                // lấy danh sách id nhà khoa học
                List<string> ScientistIds = new List<string>();
                ScientistIds.Add(data.ScientistIdOne);
                ScientistIds.Add(data.ScientistIdTwo);

                await StudentOfEvaluationBoardRef.SetSelectedRows(data.StudentId, data.Id);
                await PresidentRef.SetSelectedRows(data.PresidentId);
                await CounterattackerRef.SetSelectedRows(CounterattackerIds);
                await ScientistOfEvaluationBoardRef.SetSelectedRows(ScientistIds);
                await SecretaryOfEvaluationBoardRef.SetSelectedRows(data.SecretaryId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task CancelAsync()
        {
            activeTab = "1";
            await StudentOfEvaluationBoardRef.LoadAsync();
            await PresidentRef.LoadAsync();
            await CounterattackerRef.LoadAsync();
            await ScientistOfEvaluationBoardRef.LoadAsync();
            await SecretaryOfEvaluationBoardRef.LoadAsync();
            await CancelDetail.InvokeAsync();
        }

    }
}
