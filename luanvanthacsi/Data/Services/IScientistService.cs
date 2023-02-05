using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IScientistService
    {
        Task<List<Scientist>> GetAll();
        Task<bool> AddOrUpdateScientist(Scientist scientist);
        Task<Scientist> GetScientistByIdAsync(string id);
        Task<bool> DeleteScientistAsync(Scientist scientist);
        Task<List<Scientist>> GetListScientistBySearchAsync(string txtSearch);
      
    }
}
