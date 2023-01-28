using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IScientistService
    {
        Task<List<Scientist>> GetAll();
    }
}
