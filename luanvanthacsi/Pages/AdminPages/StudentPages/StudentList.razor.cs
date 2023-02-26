using luanvanthacsi.Data.Services;
using Microsoft.AspNetCore.Components;
using System.Runtime.CompilerServices;
using luanvanthacsi.Data.Entities;
using luanvanthacsi.Data.Data;
using luanvanthacsi.Data.Edit;
using System.Linq;
using AntDesign;
using luanvanthacsi.Data.Components;
using FluentNHibernate.Conventions;
using luanvanthacsi.Data.Extentions;
using AntDesign.TableModels;
using AutoMapper;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using Microsoft.JSInterop;
using luanvanthacsi.Ultils;
using Color = System.Drawing.Color;
using BlazorInputFile;
using OneOf.Types;
using Microsoft.AspNetCore.Components.Authorization;
using luanvanthacsi.Areas.Identity;
//using LightInject;

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentList : ComponentBase
    {
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] IFileUpload fileUpload { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        List<StudentData>? studentDatas { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<StudentData>? selectedRows;
        StudentData? selectData;
        Table<StudentData>? table;
        List<string>? ListSelectedStudentIds;
        [Inject] IMapper _mapper { get; set; }
        bool visible = false;
        bool loading = false;
        bool loadingExcel = false;
        bool showExcelForm = false;
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

        public async Task LoadAsync()
        {
            studentDatas.Clear();
            loading = true;
            visible = false;
            var students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = _mapper.Map<List<StudentData>>(list);
            loading = false;
            StateHasChanged();
        }
        void AddStudent()
        {
            var studentData = new Student();
            var lastCode = studentDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 0;
            if (lastCode != null && int.TryParse(lastCode.Substring(2), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "HV" + codeNumber.ToString("D3");
            studentData.Code = newCode;
            ShowStudentDetail(studentData);
        }

        void ShowStudentDetail(Student data)
        {
            studentEdit.LoadData(data);
            visible = true;
        }

        async Task Save(Student data)
        {
            var resultAdd = await StudentService.AddOrUpdateStudentAsync(data);
            await LoadAsync();
            if (data.Id.IsNotNullOrEmpty())
            {
                Notice.NotiSuccess("Cập nhật dữ liệu thành công");
            }
            else
            {
                Notice.NotiSuccess("Thêm dữ liệu thành công");
            }
        }

        void OnClose()
        {
            studentEdit.Close();
            visible = false;
        }

        async Task Edit(StudentData studentData)
        {
            Student student = await StudentService.GetStudentByIdAsync(studentData.Id);
            ShowStudentDetail(student);
        }

        async Task DeleteStudent(StudentData StudentData)
        {
            Student student = await StudentService.GetStudentByIdAsync(StudentData.Id.ToString());
            var result = await StudentService.DeleteStudentAsync(student);
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

        void OnRowClick(RowData<StudentData> rowData)
        {
            try
            {
                List<string> ids;
                selectData = studentDatas?.FirstOrDefault(c => c.Id == rowData.Data.Id);
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
                ListSelectedStudentIds = ids;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        async Task DeleteAsync(StudentData model = null)
        {
            try
            {
                var deleteModel = studentDatas.Where(c => selectedRows.Select(r => r.Id).Contains(c.Id)).ToList();
                List<Student> students = new List<Student>();
                // maping từ studentData thành student
                foreach (var studentData in deleteModel)
                {
                    Student student = new Student();
                    student.Id = studentData.Id;
                    students.Add(student);
                }
                var result = await StudentService.DeleteStudentListAsync(students);
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task ExportExcelAllowance()
        {
            try
            {
                var fileBase64 = Convert.ToBase64String(await GenerateExcelWorkbookAsync());
                JSRuntime.SaveAsFile(DateTime.Now.ToString("ddMMyyyy") + "danhsachhocvien-.xlsx", fileBase64);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async Task<byte[]> GenerateExcelWorkbookAsync()
        {
            try
            {
                loadingExcel = true;
                await LoadAsync();
                var stream = new MemoryStream();
                using var package = new ExcelPackage(stream);
                var workSheet = package.Workbook.Worksheets.Add("Student");

                workSheet.Row(3).Height = 22;
                workSheet.Row(1).Height = 25;
                workSheet.Row(3).Style.Font.Bold = true;
                workSheet.Row(3).Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                workSheet.Cells[1, 1, 1, 6].Merge = true;
                workSheet.Cells[1, 1, 1, 6].AutoFitColumns();
                int row = 4;
                int stt = 1;
                workSheet.Cells[1, 1, 1, 6].Value = "DANH SÁCH HỌC VIÊN BẢO VỆ LUẬN VĂN THẠC SĨ";
                workSheet.Cells[1, 1, 1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[1, 1, 1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Cells[1, 1, 1, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[1, 1, 1, 6].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(120, 139, 0));

                workSheet.Cells[3, 1].Value = "STT";
                workSheet.Cells[3, 2].Value = "Mã nhân viên";
                workSheet.Cells[3, 3].Value = "Tên nhân viên";
                workSheet.Cells[3, 4].Value = "Chức danh";
                workSheet.Cells[3, 5].Value = "Bộ phận";
                workSheet.Cells[3, 6].Value = "Ngày sinh";


                foreach (var item in studentDatas)
                {
                    workSheet.Cells[row, 1].Value = stt;
                    workSheet.Cells[row, 2].Value = item.Code;
                    workSheet.Cells[row, 3].Value = item.Name;
                    workSheet.Cells[row, 4].Value = item.PhoneNumber;
                    workSheet.Cells[row, 5].Value = item.Email;
                    workSheet.Cells[row, 6].Value = item.DateOfBirth.ToString();
                    stt++;
                    row++;
                }
                ExcelRange range = workSheet.Cells[3, 1, workSheet.Dimension.End.Row, workSheet.Dimension.End.Column];
                ExcelTable table = workSheet.Tables.Add(range, "Table1");
                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
            catch (Exception e)
            {
                throw;
            }
            loadingExcel = false;
        }

        void OpenExcelForm()
        {
            showExcelForm = true;
        }

        void OnChangeIsShow(bool isShow)
        {
            showExcelForm = isShow;
        }

        IFileListEntry file;
        public async Task LoadFile(IFileListEntry[] files)
        {
            file = files.FirstOrDefault();
            if (file != null)
            {
                await fileUpload.UploadAsync(file);
            }
        }
        public async Task InputFile()
        {
            await fileUpload.InputFile();
            file = null;
            await LoadAsync();
        }

        public void CancelImportExcelForm()
        {
            showExcelForm = false;
            file = null;
        }

       

    }
}
