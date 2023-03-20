using AntDesign.Charts;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Pages.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using static NPOI.HSSF.Util.HSSFColor;
using Title = AntDesign.Charts.Title;
namespace luanvanthacsi.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        List<StaffTypeViewModel> DataStatisticalScient { get; set; } = new List<StaffTypeViewModel>();
        List<StaffTypeViewModel> DataStatisticalStudent { get; set; } = new List<StaffTypeViewModel>();
        User CurrentUser;
        object[] data2 = new object[9999];
        bool DataStatisticalScientVisible = false;
        List<Scientist> scientistList { get; set; }
        IChartComponent Chart1;
        protected override async Task OnInitializedAsync()
        {
            string id = await getUserId();
            CurrentUser = await UserService.GetUserByIdAsync(id);
            DataStatisticalScient = new() { };
            await LoadAsync();
            scientistList = await ScientistService.GetAllByIdAsync(CurrentUser.FacultyId);
        }
        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }

        async Task LoadAsync()
        {
            DataStatisticalScient.Clear();
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
                await Chart1.ChangeData(DataStatisticalScient, true);
                await Chart1.Repaint();
                await Chart1.Render();
            }

            #endregion
            #region Thống kê thông tin học viên theo đợt bảo vệ
            List<Student> students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            DataStatisticalStudent.Add(new StaffTypeViewModel()
            {
                Value = 12,
                Type = "Đợt 1"
            });
            DataStatisticalStudent.Add(new StaffTypeViewModel()
            {
                Value = 9,
                Type = "Đợt 2"
            });
            #endregion
            object[] data02 =
                {
                    new
                    {
                        type = "Đợt 1",
                        sales = 25
                    },
                    new
                    {
                        type = "Đợt 2",
                        sales = 14
                    }
                  };
            data2 = data02;

        }

        readonly PieConfig config1 = new PieConfig
        {
            ForceFit = true,
            Title = new Title
            {
                Visible = true,
                Text = "Multicolor Pie Chart"
            },
            Description = new Description
            {
                Visible = true,
                Text = "Specify the color mapping field (colorField), and the pie slice will be displayed in different colors according to the field data. To specify the color, you need to configure the color as an array. \nWhen the pie chart label type is set to inner, the label will be displayed inside the slice. Set the offset value of the offset control label."
            },
            Radius = 0.8,
            AngleField = "value",
            ColorField = "type",
            Label = new PieLabelConfig
            {
                Visible = true,
                Type = "inner"
            }
        };

        ColumnConfig config2 = new ColumnConfig
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

        async Task OpenChartOptionFormAsync()
        {
            DataStatisticalScientVisible = true;
        }

    }
}
