using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IFacultyService
    {
        Task<List<Faculty>> GetAllAsync();
    }
}
