using AntDesign;
using BlazorInputFile;
using luanvanthacsi.Data.Components;
using luanvanthacsi.Data.Entities;
using Microsoft.AspNetCore.Components;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
//using Syncfusion.XlsIO;

namespace luanvanthacsi.Data.Services
{
    public class FileUpload : IFileUpload
    {
        [Inject] NotificationService Notice { get; set; }
        private readonly IStudentService _studentService;
        private readonly IWebHostEnvironment _environment;
        public FileUpload(IStudentService studentService, IWebHostEnvironment webHostEnvironment)
        {
            _studentService = studentService;
            _environment = webHostEnvironment;
        }
        string path;
        public async Task InputFile()
        {
            IWorkbook workbook = null;
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (path.IndexOf(".xlsx") > 0)
            {
                workbook = new XSSFWorkbook(fs);
            }
            else if (path.IndexOf(".xls") > 0)
            {
                workbook = new HSSFWorkbook(fs);
            }
            else
            {
                Notice.NotiSuccess("Xóa dữ liệu thành công");
                return;
            }

            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet != null)
            {
                int rowCount = sheet.LastRowNum;
                for (int i = 2; i <= rowCount; i++)
                {
                    Student student = new Student();
                    IRow curRow = sheet.GetRow(i);
                    student.Code = curRow.GetCell(1).StringCellValue.Trim();
                    student.Name = curRow.GetCell(2).StringCellValue.Trim();
                    student.PhoneNumber = curRow.GetCell(3).StringCellValue.Trim();
                    student.Email = curRow.GetCell(4).StringCellValue.Trim();
                    //student.DateOfBirth = curRow.GetCell(1).ToString()!= null ? DateTime.Parse(curRow.GetCell(1).ToString()) : DateTime.Now;
                    student.DateOfBirth = DateTime.Now;
                    student.CreateDate = DateTime.Now;
                    student.UpdateDate = DateTime.Now;
                    _studentService.AddOrUpdateStudentAsync(student);
                }
            }
        }

        public async Task UploadAsync(IFileListEntry file)
        {
            try
            {
                path = Path.Combine(_environment.ContentRootPath, "Upload", file.Name);
                var ms = new MemoryStream();
                await file.Data.CopyToAsync(ms);
                using (FileStream file1 = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    ms.WriteTo(file1);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
