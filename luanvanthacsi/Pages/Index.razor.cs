using AntDesign;
using AntDesign.Charts;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.AdminPages.StudentPages;
using luanvanthacsi.Pages.AdminPages.ThesisDefensepages;
using luanvanthacsi.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Umbraco.Core;
using static NPOI.HSSF.Util.HSSFColor;
using Title = AntDesign.Charts.Title;
namespace luanvanthacsi.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] IThesisDefenseService ThesisDefenseService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        List<StaffTypeViewModel> DataStatisticalScient { get; set; } = new List<StaffTypeViewModel>();
        User CurrentUser;
        object[] data2 { get; set; } = new object[100];
        bool DataStatisticalScientVisible = false;
        PieConfig ConfigStatisticalStaff;
        ColumnConfig config2;
        List<Scientist> scientistList { get; set; }
        List<ThesisDefense> ThesisDefenseList { get; set; }
        IChartComponent Chart1;
        IChartComponent Chart2;
        List<int> yearList;

        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            DataStatisticalScient = new() { };
            data2 = new object[100];
            ConfigStatisticalStaff = new PieConfig()
            {
                AppendPadding = 10,
                InnerRadius = 0.6,
                ForceFit = true,
                Radius = 0.8,
                AngleField = "value",
                ColorField = "type",
                Interactions = new Interaction[]
                   {
                        new Interaction {
                            Type = "element-selected"
                        },
                        new Interaction
                        {
                            Type = "element-active"
                        },
                        new Interaction
                        {
                            Type = "pie-statistic-active"
                        },
                   },
            };
            config2 = new ColumnConfig
            {
                ForceFit = true,
                Padding = "auto",
                XField = "type",
                YField = "sales",
                Meta = new
                {
                    Type = new
                    {
                        Alias = "Đợt bảo vệ"
                    },
                    Sales = new
                    {
                        Alias = "Số lượng học viên"
                    }
                },
                Label = new ColumnViewConfigLabel
                {
                    Visible = true,
                    Style = new TextStyle
                    {
                        FontSize = 12,
                        FontWeight = 600,
                        Opacity = 0.6,
                    }
                }
            };

            await LoadScientistAsync();
            await LoadStudentAsync();
            scientistList = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
            ThesisDefenseList = await ThesisDefenseService.GetAllByIdAsync(CurrentUser.FacultyId);
            yearList = ThesisDefenseList.Select(x => x.YearOfProtection.Year).Distinct().ToList();
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

        async Task LoadScientistAsync()
        {
            try
            {
                DataStatisticalScient = new List<StaffTypeViewModel>();
                #region thống kê thông tin hội đồng đánh giá
                List<EvaluationBoard> evaluationBoards = await EvaluationBoardService.GetAllByIdAsync(CurrentUser.FacultyId);
                // lấy tổng số chủ tịch
                int totalPresidents;
                int totalCounterattackers = 0;
                int totalSecretarys = 0;
                int totalScientists = 0;

                if (id == null)
                {
                    // lấy tổng chủ tịch
                    totalPresidents = evaluationBoards.Where(x => x.PresidentId != null).Count();

                    // lấy tổng số phản biện
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdOne != null).Count();
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdTwo != null).Count();
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdThree != null).Count();

                    // lấy tổng số Thư kí
                    totalSecretarys = evaluationBoards.Where(x => x.SecretaryId != null).Count();

                    // lấy tổng số ủy viên
                    totalScientists += evaluationBoards.Where(x => x.ScientistIdOne != null).Count();
                    totalScientists += evaluationBoards.Where(x => x.ScientistIdTwo != null).Count();
                }
                else
                {
                    // lấy tổng chủ tịch
                    totalPresidents = evaluationBoards.Where(x => x.PresidentId == id).Count();

                    // lấy tổng số phản biện
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdOne == id).Count();
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdTwo == id).Count();
                    totalCounterattackers += evaluationBoards.Where(x => x.CounterattackerIdThree == id).Count();

                    // lấy tổng số Thư kí
                    totalSecretarys = evaluationBoards.Where(x => x.SecretaryId != id).Count();

                    // lấy tổng số ủy viên
                    totalScientists += evaluationBoards.Where(x => x.ScientistIdOne == id).Count();
                    totalScientists += evaluationBoards.Where(x => x.ScientistIdTwo == id).Count();
                }
                // thực hiện change data
                DataStatisticalScient.Add(new StaffTypeViewModel()
                {
                    Value = totalPresidents,
                    Type = "Chủ tịch"
                });

                DataStatisticalScient.Add(new StaffTypeViewModel()
                {
                    Value = totalCounterattackers,
                    Type = "Phản biện"
                });

                DataStatisticalScient.Add(new StaffTypeViewModel()
                {
                    Value = totalSecretarys,
                    Type = "Thư kí"
                });

                DataStatisticalScient.Add(new StaffTypeViewModel()
                {
                    Value = totalScientists,
                    Type = "Ủy viên"
                });
                if (Chart1.IsNotNullOrEmpty())
                {
                    await Chart1.ChangeData(DataStatisticalScient);
                }
                #endregion   
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task LoadStudentAsync()
        {
            try
            {
                data2 = new object[100];
                List<ThesisDefense> thesisDefenses = await ThesisDefenseService.GetAllByIdAsync(CurrentUser.FacultyId);
                List<Student> students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);

                int currentYear;
                if (id1 == 0)
                {
                    currentYear = DateTime.Now.Year;
                }
                else
                {
                    currentYear = id1.ToInt();
                }
                var thesisDefensesOfYear = thesisDefenses.Where(x => x.YearOfProtection.Year == currentYear);
                var thesisDefenseStudentsCount = thesisDefensesOfYear
                    .Select(td => new
                    {
                        ThesisDefense = td,
                        StudentsCount = students.Count(s => s.ThesisDefenseId == td.Id)
                    });
                Array.Clear(data2, 0, data2.Length);
                int i = 0;
                foreach (var item in thesisDefenseStudentsCount)
                {
                    data2[i] = new
                    {
                        type = item.ThesisDefense.Name,
                        sales = item.StudentsCount
                    };
                    i++;
                }
                chartKey++;
            }
            catch (Exception)
            {
                throw;
            }
        }

        void OpenScientists()
        {
            NavigationManager.NavigateTo($"/scientists");
        }
        void OpenStudentList()
        {
            NavigationManager.NavigateTo($"/students");
        }
        void onpenThesisDefenses()
        {
            NavigationManager.NavigateTo($"/thesisDefenses");
        }
        void OpenEvaluationBoards()
        {
            NavigationManager.NavigateTo($"/evaluationBoards");
        }
    }
}
