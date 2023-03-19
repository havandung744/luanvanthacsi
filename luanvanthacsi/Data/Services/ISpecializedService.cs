using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface ISpecializedService
    {
        Task<List<Specialized>> GetAllAsync();
        Task<List<Specialized>> GetAllByFacultyIdAsync(string id);
        Task<Specialized> GetByIdAsync(string id);
    }
}
