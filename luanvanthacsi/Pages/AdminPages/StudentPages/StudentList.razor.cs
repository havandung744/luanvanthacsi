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
using luanvanthacsi.Ultils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using OfficeOpenXml;
using System.Data;
//using LightInject;

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentList : ComponentBase
    {
        [Inject] ExcelExporter ExcelExporter { get; set; }
        [Inject] Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [Inject] AuthenticationStateProvider _authenticationStateProvider { get; set; }
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] AntDesign.NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        [Inject] IFacultyService FacultyService { get; set; }
        [Inject] IUserService UserService { get; set; }
        [Inject] IScientistService ScientistService { get; set; }
        [Inject] IJSRuntime JSRuntime { get; set; }
        [Inject] IMapper _mapper { get; set; }
        List<StudentData>? studentDatas { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        IEnumerable<StudentData>? selectedRows;
        StudentData? selectData;
        Table<StudentData>? table;
        List<string>? ListSelectedStudentIds;
        bool visible = false;
        bool loading = false;
        bool showExcelForm = false;
        User CurrentUser;
        bool importVisible = false;
        bool existModalVisible = false;
        string facultyId;
        List<Faculty> facultyList { get; set; }
        List<Scientist> scientistList { get; set; }

        List<Student> ExcelStudentDatas { get; set; }

        ImportExcelForm importExcelFormRef;
        List<ExcelSheetObject> Sheets { get; set; }

        void ImportExcelCancel(bool val)
        {
            importVisible = val;
        }

        void CancelImport()
        {
            existModalVisible = false;
        }


        void GetTemplateFileAsync()
        {
            try
            {
                var fileBase64 = Convert.ToBase64String(GenerateTemplateExcel());
                JSRuntime.SaveAsFile(DateTime.Now.ToString("ddMMyyyy") + "-DanhMucChucDanh.xlsx", fileBase64);
            }
            catch (Exception)
            {
                throw;
            }
        }

        byte[] GenerateTemplateExcel()
        {
            try
            {

                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Excel\\Template", "DanhSachHocVien.xlsx");
                using (var stream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
                {
                    var package = new ExcelPackage(stream);
                    return package.GetAsByteArray();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<string> getUserId()
        {
            var user = (await _authenticationStateProvider.GetAuthenticationStateAsync()).User;
            var UserId = user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
            return UserId;
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                string id = await getUserId();
                CurrentUser = await UserService.GetUserByIdAsync(id);
                studentDatas = new();
                facultyList = await FacultyService.GetAllAsync();
                scientistList = await ScientistService.GetAll();
                await LoadAsync();
                Sheets = new List<ExcelSheetObject> { new ExcelSheetObject("HocVien", "KEY_STAFFIMPORT", 6, null, GetTable().GetDataColumns(), 5) };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable GetTable()
        {
            try
            {
                var table = new DataTable();
                var properties = typeof(Student).GetProperties();
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
            studentDatas?.Clear();
            loading = true;
            visible = false;
            try
            {
                facultyId = await localStorage.GetItemAsync<string>("facultyIdOfStudent");
            }
            catch
            {
                facultyId = null;
            }
            List<Student> students = new List<Student>();
            if (CurrentUser.FacultyId == null)
            {
                students = await StudentService.GetAllByIdAsync(facultyId);
            }
            else
            {
                students = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
            }
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            foreach (Student item in list)
            {
                Scientist studentObj1 = scientistList?.FirstOrDefault(s => s.Id == item.InstructorIdOne);
                Scientist studentObj2 = scientistList?.FirstOrDefault(s => s.Id == item.InstructorIdTwo);
                if (studentObj1 != null)
                {
                    item.InstructorNameOne = studentObj1?.Name;
                }
                if (studentObj2 != null)
                {
                    item.InstructorNameTwo = studentObj2?.Name;
                }
            }
            studentDatas = _mapper.Map<List<StudentData>>(list);
            int stt = 1;
            studentDatas.ForEach(x => { x.stt = stt++; });
            loading = false;
            StateHasChanged();
        }

        string GetNewCode()
        {
            var lastCode = studentDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 1;
            if (lastCode != null && int.TryParse(lastCode.Substring(2), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "HV" + codeNumber.ToString("D3");
            return newCode;
        }

        void AddStudent()
        {
            var studentData = new Student();
            var lastCode = studentDatas?.OrderByDescending(x => x.Code).Select(x => x.Code).FirstOrDefault();
            int codeNumber = 1;
            if (lastCode != null && int.TryParse(lastCode.Substring(2), out codeNumber))
            {
                codeNumber++;
            }
            string newCode = "HV" + codeNumber.ToString("D3");
            studentData.Code = newCode;
            ShowStudentDetail(studentData);
        }

        async Task ShowStudentDetail(Student data)
        {
            await studentEdit.LoadData(data);
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
            await ShowStudentDetail(student);
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
            catch (Exception)
            {
                throw;
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
        public string GetFileMauUrl()
        {
            return "/template/DanhMucChucDanh.xlsx";
        }

        async Task ShowImport()
        {
            try
            {
                await importExcelFormRef.InitAsync();
                importExcelFormRef.LoaiExcelImport = Data.Components.Enum.LoaiExcelImport.StaffProfile;
                importExcelFormRef.Sheets = Sheets;
                importExcelFormRef.FileMauUrl = GetFileMauUrl();
                importVisible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Student> ConvertStudent(DataTable result)
        {
            List<Student> students = new List<Student>();

            if (result.Rows.Count > 0)
            {
                for (int i = 0; i < result.Rows.Count; i++)
                {
                    Student student = new Student()
                    {
                        Id = ObjectExtentions.GenerateGuid(),
                        FacultyId = CurrentUser.FacultyId,
                    };
                    #region convert data

                    student.Code = result.Rows[i][nameof(Student.Code)].IsNotNullOrEmpty() ? result.Rows[i][nameof(Student.Code)].ToString() : GetNewCode();
                    student.Name = result.Rows[i][nameof(Student.Name)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.Name)].ToString() : student.Name;
                    student.Email = result.Rows[i][nameof(Student.Email)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.Email)].ToString() : student.Email;
                    student.PhoneNumber = result.Rows[i][nameof(Student.PhoneNumber)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.PhoneNumber)].ToString() : student.PhoneNumber;
                    student.TopicName = result.Rows[i][nameof(Student.TopicName)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.TopicName)].ToString() : student.TopicName;
                    student.InstructorIdOne = result.Rows[i][nameof(Student.InstructorIdOne)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.InstructorIdOne)].ToString() : student.InstructorIdOne;
                    student.InstructorIdTwo = result.Rows[i][nameof(Student.InstructorIdTwo)].IsNotNullOrEmpty() ? result.Rows[i][nameof(student.InstructorIdTwo)].ToString() : student.InstructorIdTwo;
                    student.DateOfBirth = result.Rows[i][nameof(Student.DateOfBirth)].IsNotNullOrEmpty() ? Convert.ToDateTime(result.Rows[i][nameof(Student.DateOfBirth)]) : student.DateOfBirth;
                    #endregion
                    students.Add(student);
                }
            }

            return students;
        }

        async Task SaveAndUpdateAsync()
        {
            try
            {
                bool save;
                if (CurrentUser.FacultyId == null)
                {
                    save = await StudentService.AddOrUpdateStudentListAsync(ExcelStudentDatas, facultyId);
                }
                else
                {
                    save = await StudentService.AddOrUpdateStudentListAsync(ExcelStudentDatas, CurrentUser.FacultyId);
                }
                if (save == true)
                {
                    Notice.NotiSuccess("Thêm mới và cập nhật danh sách học viên từ Excel thành công.");
                    await LoadAsync();
                }
                else
                {
                    Notice.NotiWarning("Thêm mới và cập nhật danh sách học viên từ Excel thất bại.");
                }
                existModalVisible = false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task SaveListImportAsync()
        {
            try
            {
                if (ExcelStudentDatas?.Any() == true)
                {
                    var listCodeExcel = ExcelStudentDatas.Select(x => x.Code).ToList();
                    bool checkValidate = studentDatas.Any(x => listCodeExcel.Contains(x.Code));
                    if (!checkValidate)
                    {
                        var save = await StudentService.AddListStudentAsync(ExcelStudentDatas);
                        if (save == true)
                        {
                            Notice.NotiSuccess("Import Excel thành công.");
                            await LoadAsync();
                        }
                        else
                        {
                            Notice.NotiWarning("Import Excel thất bại.");
                        }
                    }
                    else
                    {
                        existModalVisible = true;
                    }
                    importVisible = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ImportFromFileAsync(DataSet dataSet, string maPB)
        {
            try
            {
                DataTable dataStudent = dataSet.Tables[0];
                ExcelStudentDatas = ConvertStudent(dataStudent);
                await SaveListImportAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task ExportExcelAsync()
        {
            try
            {
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "Excel\\Template", "DanhSachHocVien.xlsx");
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
                        List<Student> data = new List<Student>();
                        if (CurrentUser.FacultyId != null)
                        {
                            data = await StudentService.GetAllByIdAsync(CurrentUser.FacultyId);
                        }
                        else
                        {
                            data = await StudentService.GetAllByIdAsync(facultyId);
                        }
                        if (data.Any() == true)
                        {
                            List<StudentExportExcel> studentExportExcels = new List<StudentExportExcel>();
                            studentExportExcels = _mapper.Map<List<StudentExportExcel>>(data);
                            int stt = 1;
                            foreach (var item in studentExportExcels)
                            {
                                item.stt = stt;
                                stt++;
                            }

                            ExcelExporter.WriteToSheet(studentExportExcels, wSheet, Sheets.First());
                        }
                        else
                        {
                            Notice.NotiError("Không có dữ liệu!");
                            return;
                        }
                        //package.Workbook.CalcMode = ExcelCalcMode.Automatic;
                        var fileBase64 = Convert.ToBase64String(package.GetAsByteArray());
                        JSRuntime.SaveAsFile(DateTime.Now.ToString("ddMMyyyy-HHmmss") + "-DanhSachHocVien.xlsx", fileBase64);
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
            await localStorage.SetItemAsync("facultyIdOfStudent", facultyId);
            await LoadAsync();
        }
    }
}
