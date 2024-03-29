﻿using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<List<Student>> GetAllByIdAsync(string id);
        Task<bool> AddOrUpdateStudentAsync(Student student);
        Task<bool> AddOrUpdateStudentListAsync(List<Student> students, string facultyId);
        Task<Student> GetStudentByIdAsync(string id);
        Task<bool> DeleteStudentAsync(Student student);
        Task<bool> DeleteStudentListAsync(List<Student> student);
        Task<List<Student>> GetListStudentBySearchAsync(string txtSearch);
        Task<bool> AddListStudentAsync(List<Student> students);

    }
}
