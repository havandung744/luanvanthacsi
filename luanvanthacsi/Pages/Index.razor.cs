using AntDesign.Charts;
using luanvanthacsi.Pages.Shared;
using Microsoft.AspNetCore.Components;
using static NPOI.HSSF.Util.HSSFColor;

namespace luanvanthacsi.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] NavigationManager NavigationManager { get; set; }
        object[] data1 = new object[9999];
        protected override async Task OnInitializedAsync()
        {
            object[] data01 =
                    {
                    new
                    {
                        type = "Giỏi",
                        sales = 6
                    },
                    new
                    {
                        type = "Khá",
                        sales = 9
                    },
                    new
                    {
                        type = "Trung bình",
                        sales = 1
                    },
                    new
                    {
                        type = "Yếu",
                        sales = 3
                    },
                  };
            data1 = data01;
        }

        ColumnConfig config2 = new ColumnConfig
        {
            //Title = new Title
            //{
            //    Visible = true,
            //    Text = "基础柱状图-图形标签"
            //},
            //Description = new Description
            //{
            //    Visible = true,
            //    Text = "基础柱状图图形标签默认位置在柱形上部。",
            //},
            ForceFit = true,
            Padding = "auto",
            XField = "type",
            YField = "sales",
            Meta = new
            {
                Type = new
                {
                    Alias = "Xếp hạng học lực"
                },
                Sales = new
                {
                    Alias = "Số lượng"
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

    }
}
