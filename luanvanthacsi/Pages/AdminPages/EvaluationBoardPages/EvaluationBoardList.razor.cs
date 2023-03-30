using AntDesign;
using AntDesign.TableModels;
using AutoMapper;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Extentions;
using luanvanthacsi.Data.Services;
using luanvanthacsi.Excel;
using luanvanthacsi.Excel.ClassExcel;
using luanvanthacsi.Models;
using luanvanthacsi.Ultils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.Data;
using static luanvanthacsi.Data.Components.Enum;

namespace luanvanthacsi.Pages.AdminPages.EvaluationBoardPages
{
    public partial class EvaluationBoardList : ComponentBase
    {
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] ExcelExporter ExcelExporter { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IEvaluationBoardService EvaluationBoardService { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IFacultyService FacultyService { get; set; }
        [Inject] IMapper _mapper { get; set; }
        [Inject] ISpecializedService SpecializedService { get; set; }
        [CascadingParameter] SessionData SessionData { get; set; }
        List<EvaluationBoardData>? evaluationBoardDatas { get; set; } = new();
        List<Scientist>? Scientists { get; set; } = new();
        bool visible = false;
        EvaluationBoardEdit evaluationBoardEdit = new EvaluationBoardEdit();
        bool loading = false;
        IEnumerable<EvaluationBoardData>? selectedRows;
        EvaluationBoardData? selectData;
        Table<EvaluationBoardData>? table;
        List<string>? ListSelectedEvaluationBoardDataIds;
        bool addVisible;
        List<Faculty> facultyList { get; set; }
        List<ExcelSheetObject> Sheets { get; set; }
        EvaluationBoardAddLayout EvaluationBoardAddLayoutRef { get; set; } = new();
        string facultyId;

        protected override async Task OnInitializedAsync()
        {
            evaluationBoardDatas = new();
            facultyList = await FacultyService.GetAllAsync();
            Sheets = new List<ExcelSheetObject> { new ExcelSheetObject("HoiDong", "KEY_STAFFIMPORT", 4, null, GetTable().GetDataColumns(), 3) };
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadAsync();
            }
        }

        public DataTable GetTable()
        {
            try
            {
                var table = new DataTable();
                var properties = typeof(EvaluationBoardExcel).GetProperties();
                foreach (var p in properties)
                {
                    table.Columns.Add(p.Name, Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType);
                }
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LoadAsync()
        {
            try
            {
                loading = true;
                StateHasChanged();
                visible = false;
                evaluationBoardDatas?.Clear();
                List<EvaluationBoard> evaluationBoards = new List<EvaluationBoard>();

                if (SessionData.CurrentUser?.FacultyId == null)
                {
                    facultyId = await localStorage.GetItemAsync<string>("facultyIdOfEvaluation");
                    evaluationBoards = await EvaluationBoardService.GetAllByIdAsync(facultyId);
                }
                else
                {
                    evaluationBoards = await EvaluationBoardService.GetAllByIdAsync(SessionData.CurrentUser.FacultyId);
                }
                // hiển thị dữ liệu mới nhất lên đầu trang
                var list = evaluationBoards.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
                evaluationBoardDatas = _mapper.Map<List<EvaluationBoardData>>(list);
                #region join bảng lấy dữ liệu
                //List<Scientist> scientists = new List<Scientist>();
                if (SessionData.CurrentUser?.FacultyId == null)
                {
                    Scientists = await ScientistService.GetAllByIdAsync(facultyId);
                }
                else
                {
                    Scientists = await ScientistService.GetAllByIdAsync(SessionData.CurrentUser.FacultyId);
                }
                foreach (var item in evaluationBoardDatas)
                {
                    // lấy thông tin student
                    Student student = await StudentService.GetStudentByIdAsync(item.StudentId);
                    item.StudentName = student?.Name;
                    item.TopicName = student?.TopicName;
                    item.DOB = student?.DateOfBirth;
                    item.Branch = Scientists.Where(x => x.Id == item.PresidentId).Select(x => x.Name).First();
                    // lấy thông tin hướng dẫn 1
                    item.InstructorNameOne = Scientists.Where(x => x.Id == student?.InstructorIdOne).Select(x => x.Name).FirstOrDefault();
                    // lấy thông tin hướng dẫn 2
                    item.InstructorNameTwo = Scientists.Where(x => x.Id == student?.InstructorIdTwo).Select(x => x.Name).FirstOrDefault();
                    // lấy thông tin chủ tịch
                    item.PresidentName = Scientists.Where(x => x.Id == item.PresidentId).Select(x => x.Name).First();
                    // lấy thông tin phản biện 1
                    item.CounterattackerOne = Scientists.Where(x => x.Id == item.CounterattackerIdOne).Select(x => x.Name).First();
                    // lấy thông tin phản biện 2
                    item.CounterattackerTwo = Scientists.Where(x => x.Id == item.CounterattackerIdTwo).Select(x => x.Name).First();
                    // lấy thông tin phản biện 3
                    item.CounterattackerThree = Scientists.Where(x => x.Id == item.CounterattackerIdThree).Select(x => x.Name).First();
                    // lấy thông tin phản biện thư kí
                    item.SecretaryName = Scientists.Where(x => x.Id == item.SecretaryId).Select(x => x.Name).First();
                    // lấy thông tin ủy viên 1
                    item.ScientistOne = Scientists.Where(x => x.Id == item.ScientistIdOne).Select(x => x.Name).First();
                    // lấy thông tin ủy viên 2
                    item.ScientistTwo = Scientists.Where(x => x.Id == item.ScientistIdTwo).Select(x => x.Name).First();
                }
                #endregion
                int stt = 1;
                evaluationBoardDatas?.ForEach(x => { x.stt = stt++; });
                loading = false;
                StateHasChanged();
            }
            catch (Exception)
            {
                throw;
            }

        }

        async Task AddEvaluationBoard()
        {
            await EvaluationBoardAddLayoutRef.LoadAddAsync();
            addVisible = true;
        }

        async Task ShowEvaluationBoardDetail(EvaluationBoard data)
        {
            await EvaluationBoardAddLayoutRef.LoadDetail(data);
            addVisible = true;
        }

        async Task Save(EvaluationBoard data)
        {
            addVisible = false;
            var resultAdd = await EvaluationBoardService.AddOrUpdateEvaluationBoard(data);
            if (resultAdd == true)
            {
                if (data.Id.IsNotNullOrEmpty())
                {
                    Notice.NotiSuccess("Cập nhật dữ liệu thành công");
                }
                else
                {
                    Notice.NotiSuccess("Thêm dữ liệu thành công");
                }
                await LoadAsync();
            }
            else
            {
                if (data.Id.IsNotNullOrEmpty())
                {
                    Notice.NotiError("Cập nhật dữ liệu thất bại");
                }
                else
                {
                    Notice.NotiError("Thêm dữ liệu thất bại");
                }
            }

        }

        void OnClose()
        {
            evaluationBoardEdit.Close();
            visible = false;
        }

        async Task Edit(EvaluationBoardData EvaluationBoardData)
        {
            EvaluationBoard evaluationBoard = await EvaluationBoardService.GetEvaluationBoardByIdAsync(EvaluationBoardData.Id);
            await ShowEvaluationBoardDetail(evaluationBoard);
        }



        async Task DeleteEvaluationBoard(EvaluationBoardData evaluationBoardData)
        {
            EvaluationBoard evaluationBoard = await EvaluationBoardService.GetEvaluationBoardByIdAsync(evaluationBoardData.Id.ToString());
            var result = await EvaluationBoardService.DeleteEvaluationBoardAsync(evaluationBoard);
            if (result.Equals(true))
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                await LoadAsync();
            }
            else
            {
                Notice.NotiError("Xóa dữ liệu thất bại");
            }
        }

        void OnRowClick(RowData<EvaluationBoardData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = evaluationBoardDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
                ids = selectedRows != null ? selectedRows.Select(c => c.Id).ToList() : new();
                if (ids.Contains(selectData.Id))
                {
                    ids.Remove(selectData.Id);
                }
                else
                {
                    ids.Add(selectData.Id);
                }
                table?.SetSelection(ids.ToArray());
                ListSelectedEvaluationBoardDataIds = ids;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task DeleteAsync(EvaluationBoardData model = null)
        {
            try
            {
                var deleteModel = evaluationBoardDatas.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<EvaluationBoard> evaluationBoards = new List<EvaluationBoard>();
                // maping từ studentData thành student
                foreach (var evaluationBoardData in deleteModel)
                {
                    EvaluationBoard evaluationBoard = new EvaluationBoard();
                    evaluationBoard.Id = evaluationBoardData.Id;
                    evaluationBoards.Add(evaluationBoard);
                }
                var result = await EvaluationBoardService.DeleteEvaluationBoardAsync(evaluationBoards);
                if (result.Equals(true))
                {
                    Notice.NotiSuccess("Xóa dữ liệu thành công");
                    await LoadAsync();
                }
                else
                {
                    Notice.NotiError("Xóa dữ liệu thất bại");
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        void CancelDetail()
        {
            addVisible = false;
        }

        async Task ExportExcelAsync()
        {
            try
            {
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Excel\\Template", "HoiDongBaoVe.xlsx");
                using (var stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                {
                    var package = new ExcelPackage(stream);
                    try
                    {
                        ExcelWorksheet wSheet;
                        try
                        {
                            wSheet = package.Workbook.Worksheets[Sheets.First().Name];
                        }
                        catch (Exception)
                        {
                            wSheet = package.Workbook.Worksheets[Sheets.First().Name];
                        }
                        var data = _mapper.Map<List<EvaluationBoardExcel>>(evaluationBoardDatas);
                        if (data.Any() == true)
                        {
                            ExcelExporter.WriteToSheet(data, wSheet, Sheets.First());
                        }
                        else
                        {
                            Notice.NotiError("Không có dữ liệu!");
                            return;
                        }
                        //package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                        var fileBase64 = Convert.ToBase64String(package.GetAsByteArray());
                        JSRuntime.SaveAsFile(DateTime.Now.ToString("ddMMyyyy-HHmmss") + "-HoiDongBaoVe.xlsx", fileBase64);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        async Task ChangeFacultyId()
        {
            await localStorage.SetItemAsync("facultyIdOfEvaluation", facultyId);
            await LoadAsync();
        }

        async Task ExportDocxAsync(EvaluationBoardData dt)
        {
            try
            {
                #region get Data

                var counterAttachIds = new List<string> { dt.CounterattackerIdOne, dt.CounterattackerIdTwo, dt.CounterattackerIdThree };
                var scientistIds = new List<string> { dt.ScientistIdOne, dt.ScientistIdTwo };

                var counters = Scientists.Where(c => counterAttachIds.Contains(c.Id)).ToList();
                var scientists = Scientists.Where(c => scientistIds.Contains(c.Id)).ToList();
                var president = Scientists.Where(c => c.Id == dt.PresidentId).ToList();
                var secretary = Scientists.Where(c => c.Id == dt.SecretaryId).ToList();

                counters.ForEach(c => c.EvaluationRole = EvaluationRole.CounterAttack);
                scientists.ForEach(c => c.EvaluationRole = EvaluationRole.Scientist);
                president.ForEach(c => c.EvaluationRole = EvaluationRole.President);
                secretary.ForEach(c => c.EvaluationRole = EvaluationRole.Secretary);

                var listEvaluationBoard = counters.Concat(scientists).Concat(president).Concat(secretary).ToList();
                var listEvaluationBoardAll = counters.Concat(scientists).Concat(president).Concat(secretary).ToList();
                listEvaluationBoardAll.Add(new()
                {
                    //Name = dt.InstructorOne,
                    Name = dt.InstructorNameOne,
                    WorkingAgency = "Đơn vị TEST",
                    EvaluationRole = EvaluationRole.Instructor
                });

                int i = 1;
                int j = 1;
                #endregion

                EvaluationBoardDocx documentData = new();
                Student student = await StudentService.GetStudentByIdAsync(dt.StudentId);
                documentData.Content = new EvaluationBoardDocx.Data
                {
                    DateForm = DateTime.Now.FormatDayMonthYear(),
                    StudentName = dt.StudentName,
                    DOB = dt.DOB.ToShortDate(),
                    TopicName = dt.TopicName,
                    FacultyName = facultyList?.Where(x => x.Id == dt?.FacultyId).Select(x => x.Name).FirstOrDefault(),
                    FacultyCode = facultyList?.Where(x => x.Id == dt?.FacultyId).Select(x => x.Code).FirstOrDefault(),
                    InstructorName = dt.InstructorNameOne,
                    BoardTotal = listEvaluationBoard.Count().ToString(),
                    EvaluationBoards = _mapper.Map<List<EvaluationBoardDocx.EvaluationBoard>>(listEvaluationBoard.OrderBy(c => c.EvaluationRole.EnumToInt())),
                    EvaluationBoardAll = _mapper.Map<List<EvaluationBoardDocx.EvaluationBoard>>(listEvaluationBoardAll.OrderBy(c => c.EvaluationRole.EnumToInt()))
                };

                if (documentData.Content.EvaluationBoards.Any())
                {
                    foreach (var item in documentData.Content.EvaluationBoards)
                    {
                        item.No = i++.ToString();
                        item.Name = $"{item.Degree.FormatDegree()} {item.Name}";
                    }
                }
                if (documentData.Content.EvaluationBoardAll.Any())
                {
                    foreach (var item in documentData.Content.EvaluationBoardAll)
                    {
                        item.No = j++.ToString();
                        item.Name = $"{item.Degree.FormatDegree()} {item.Name}";
                    }
                }

                var fileBytes = WordUltil.WriteDOCX("EvaluationBoardDocxTemplate.docx", documentData);
                var fileBase64 = Convert.ToBase64String(fileBytes);
                JSRuntime.SaveAsFile(DateTime.Now.ToString("ddMMyyyy-HHmmss") + "-HoiDongBaoVe.docx", fileBase64);

            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
