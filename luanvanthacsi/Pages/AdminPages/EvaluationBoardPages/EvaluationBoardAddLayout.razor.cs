using AntDesign;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Models;
using Microsoft.AspNetCore.Components;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardAddLayout : ComponentBase
    {
        [Inject] NotificationService Notice { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [CascadingParameter] public SessionData SessionData { get; set; }
        [Parameter] public EventCallback<EvaluationBoard> SaveChange { get; set; }
        [Parameter] public EventCallback CancelDetail { get; set; }
        public StudentOfEvaluationBoard StudentOfEvaluationBoardRef { get; set; } = new();
        public PresidentOfEvaluationBoard PresidentRef { get; set; } = new();
        public CounterattackerOfEvaluationBoard CounterattackerRef { get; set; } = new();
        public SecretaryOfEvaluationBoard SecretaryOfEvaluationBoardRef { get; set; } = new();
        public ScientistOfEvaluationBoard ScientistOfEvaluationBoardRef { get; set; } = new();
        List<string> selectedScientistIds;
        private string activeTab = "1";
        string idUpdate = "";

        protected override async Task OnInitializedAsync()
        {
            selectedScientistIds = new List<string>();
        }

        void OnTabChange(string key)
        {
            selectedScientistIds.Clear();
            //if (key != "1")
            //{
            //    string userId = StudentOfEvaluationBoardRef.GetStudentId();
            //    if (userId == null)
            //    {
            //        activeTab = "1";
            //        Notice.NotiWarning("Vui lòng chọn học viên.");
            //        return;
            //    }
            //}
            if (key != "2")
            {
                var presidentId = PresidentRef.GetId();
                //if (presidentId.Count == 0 && key != "1")
                //{
                //    activeTab = "2";
                //    Notice.NotiWarning("Vui lòng chọn chủ tịch.");
                //    return;
                //}
                //else
                //{
                if (presidentId.FirstOrDefault().IsNotNullOrEmpty())
                {
                    selectedScientistIds.Add(presidentId?.FirstOrDefault());
                }
                //}
            }
            if (key != "3")
            {
                var counterattackerRef = CounterattackerRef.GetId();
                //if (counterattackerRef.Count != 3 && key != "1" && key != "2")
                //{
                //    activeTab = "3";
                //    Notice.NotiWarning("Vui lòng chọ 3 phản biện.");
                //    return;
                //}
                //else
                //{
                foreach (var item in counterattackerRef)
                {
                    selectedScientistIds.Add(item);
                }
                //}
            }
            if (key != "4")
            {
                var secretaryOfEvaluationBoardRef = SecretaryOfEvaluationBoardRef.GetId();
                //if (secretaryOfEvaluationBoardRef.Count == 0 && key != "1" && key != "2" && key != "3")
                //{
                //    activeTab = "4";
                //    Notice.NotiWarning("Vui lòng chọn thư ký.");
                //    return;
                //}
                //else
                //{
                if (secretaryOfEvaluationBoardRef.FirstOrDefault().IsNotNullOrEmpty())
                {
                    selectedScientistIds.Add(secretaryOfEvaluationBoardRef?.FirstOrDefault());
                }
                //}
            }
            if (key != "5")
            {
                var scientistOfEvaluationBoardRef = ScientistOfEvaluationBoardRef.GetId();
                //if (scientistOfEvaluationBoardRef.Count != 2 && key != "1" && key != "2" && key != "3" && key != "4")
                //{
                //    activeTab = "5";
                //    Notice.NotiWarning("Vui lòng chọn 2 uỷ viên.");
                //    return;
                //}
                //else
                //{
                foreach (var item in scientistOfEvaluationBoardRef)
                {
                    selectedScientistIds.Add(item);
                }
                //}
            }
            activeTab = key;
        }

        async Task SaveAsync()
        {
            try
            {
                EvaluationBoard evaluationBoard = new EvaluationBoard();
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
                if (SessionData.CurrentUser?.FacultyId == null)
                {
                    string facultyId = await localStorage.GetItemAsync<string>("facultyIdOfEvaluation");
                    evaluationBoard.FacultyId = facultyId;
                    evaluationBoard.Status = 2;
                }
                else
                {
                    evaluationBoard.FacultyId = SessionData.CurrentUser.FacultyId;
                    evaluationBoard.Status = 1;
                }
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
                await SecretaryOfEvaluationBoardRef.SetSelectedRows(SecretaryId);
                await ScientistOfEvaluationBoardRef.SetSelectedRows(ScientistIds);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LoadAddAsync()
        {
            await StudentOfEvaluationBoardRef.LoadAsync();
            await PresidentRef.LoadAsync();
            await CounterattackerRef.LoadAsync();
            await ScientistOfEvaluationBoardRef.LoadAsync();
            await SecretaryOfEvaluationBoardRef.LoadAsync();
        }

        async Task CancelAsync()
        {
            await CancelDetail.InvokeAsync();
            activeTab = "1";
            idUpdate = "";
            selectedScientistIds.Clear();
        }

    }
}
