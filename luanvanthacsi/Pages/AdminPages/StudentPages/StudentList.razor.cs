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

namespace luanvanthacsi.Pages.AdminPages.StudentPages
{
    public partial class StudentList : ComponentBase
    {
        [Inject] TableLocale TableLocale { get; set; }
        [Inject] NotificationService Notice { get; set; }
        [Inject] IStudentService StudentService { get; set; }
        List<StudentData>? studentDatas { get; set; }
        List<Student>? students { get; set; }
        StudentEdit studentEdit = new StudentEdit();
        TaskSearchStudent taskSearchStudent = new TaskSearchStudent();
        IEnumerable<StudentData>? selectedRows;
        StudentData? selectData;
        Table<StudentData>? table;
        List<string>? ListSelectedStudentIds;
        readonly IMapper _mapper;
        bool visible = false;
        bool loading = false;
        string txtValue { get; set; }
        protected override async Task OnInitializedAsync()
        {

            studentDatas = new();
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            studentDatas.Clear();
            loading = true;
            visible = false;
            StateHasChanged();
            var students = await StudentService.GetAllAsync();
            // hiển thị dữ liệu mới nhất lên đầu trang
            var list = students.OrderByDescending(x => x.UpdateDate).ThenByDescending(x => x.UpdateDate).ToList();
            studentDatas = GetViewModels(list);
            loading = false;
            StateHasChanged();
        }

        List<StudentData> GetViewModels(List<Student> datas)
        {
            var models = new List<StudentData>();
            StudentData model;
            int stt = 1;
            datas.ForEach(c =>
            {
                model = new StudentData();
                model.Id = c.Id;
                model.stt = stt;
                model.Name = c.Name;
                model.Email = c.Email;
                model.Code = c.Code;
                model.PhoneNumber = c.PhoneNumber;
                model.DateOfBirth = c.DateOfBirth;
                model.CreateDate = c.CreateDate;
                model.UpdateDate = c.UpdateDate;
                models.Add(model);
                stt++;
            });
            return models;
        }

        void AddStudent()
        {
            var studentData = new Student();
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

        async Task Search()
        {
            var txtSearch = taskSearchStudent.txtSearch;
            studentDatas?.Clear();
            students = await StudentService.GetListStudentBySearchAsync(txtSearch);
            studentDatas = GetViewModels(students);
            StateHasChanged();
        }

        public class TaskSearchStudent
        {
            public string? txtSearch { get; set; }
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
                var deleteModel = studentDatas.Where(c=> selectedRows.Select(r=>r.Id).Contains(c.Id)).ToList();
                List<Student> students= new List<Student>();
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
    }
}
