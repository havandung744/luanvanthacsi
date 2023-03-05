using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface ILecturersService
    {
        Task<List<Lecturers>> GetAllByIdAsync(string id);
    }
}
