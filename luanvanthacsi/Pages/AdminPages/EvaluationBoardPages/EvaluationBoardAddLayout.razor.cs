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
        public ScientistOfEvaluationBoard PresidentRef { get; set; } = new();
        ScientistOfEvaluationBoard CounterattackerRef { get; set; } = new();
        ScientistOfEvaluationBoard ScientistOfEvaluationBoardRef { get; set; } = new();
        ScientistOfEvaluationBoard SecretaryOfEvaluationBoardRef { get; set; } = new();
        [Parameter] public EventCallback<EvaluationBoard> SaveChange { get; set; }
        [Parameter] public EventCallback CancelDetail { get; set; }
        private string activeTab = "1";
        string idUpdate = "";
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
                var presidentId = PresidentRef.GetId();
                if (presidentId.Count() == 0)
                {
                    Notice.NotiError("Vui lòng chọn chủ tịch hội đồng!");
                    return;
                }
                else
                {
                    evaluationBoard.PresidentId = presidentId.First();
                }
                // lấy danh sách id của giảng viên phản biện
                var counterattackerIds = CounterattackerRef.GetId();
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
                var scientistIds = ScientistOfEvaluationBoardRef.GetId();
                if (scientistIds.Count != 2)
                {
                    Notice.NotiError("Vui lòng chọn hai ủy viên!");
                    return;
                }
                else
                {
                    evaluationBoard.ScientistIdOne = scientistIds[0];
                    evaluationBoard.ScientistIdTwo = scientistIds[1];
                }
                // lấy id của thư ký
                var secretaryId = SecretaryOfEvaluationBoardRef.GetId();
                if (secretaryId.Count() == 0)
                {
                    Notice.NotiError("Vui lòng chọn thư ký!");
                    return;
                }
                else
                {
                    evaluationBoard.SecretaryId = secretaryId.First();
                }
                // Thực hiện lưu
                activeTab = "1";
                await StudentOfEvaluationBoardRef.LoadAsync();
                await PresidentRef.LoadAsync();
                await CounterattackerRef.LoadAsync();
                await ScientistOfEvaluationBoardRef.LoadAsync();
                await SecretaryOfEvaluationBoardRef.LoadAsync();
                await CancelDetail.InvokeAsync();
                evaluationBoard.FacultyId = CurrentUser.FacultyId;
                evaluationBoard.Id = idUpdate;
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
                idUpdate = data.Id;
                // lấy danh sách id phản biện
                List<string> CounterattackerIds = new List<string>();
                CounterattackerIds.Add(data.CounterattackerIdOne);
                CounterattackerIds.Add(data.CounterattackerIdTwo);
                CounterattackerIds.Add(data.CounterattackerIdThree);

                // lấy danh sách id nhà khoa học
                List<string> ScientistIds = new List<string>();
                ScientistIds.Add(data.ScientistIdOne);
                ScientistIds.Add(data.ScientistIdTwo);

                // lấy id chủ tịch hội đồng
                List<string> PrersidentId = new List<string>();
                PrersidentId.Add(data.PresidentId);
                // lấy id thư ký hội đồng
                List<string> SecretaryId = new List<string>();
                SecretaryId.Add(data.SecretaryId);


                await StudentOfEvaluationBoardRef.SetSelectedRows(data.StudentId, data.Id);
                await PresidentRef.SetSelectedRows(PrersidentId);
                await CounterattackerRef.SetSelectedRows(CounterattackerIds);
                await ScientistOfEvaluationBoardRef.SetSelectedRows(ScientistIds);
                await SecretaryOfEvaluationBoardRef.SetSelectedRows(SecretaryId);
            }
            catch (Exception)
            {
                throw;
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
