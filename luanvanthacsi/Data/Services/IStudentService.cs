using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync();
        Task<bool> AddOrUpdateStudentAsync(Student student);
        Task<Student> GetStudentByIdAsync(string id);
        Task<bool> DeleteStudentAsync(Student student);
        Task<bool> DeleteStudentListAsync(List<Student> student);
        Task<List<Student>> GetListStudentBySearchAsync(string txtSearch);
    }
}
