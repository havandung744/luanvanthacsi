using luanvanthacsi.Api.Entities;

namespace luanvanthacsi.Api.Repositores
{
    public interface IScientistRepository
    {
        Task<IEnumerable<Scientist>> GetScientistsAsync();
        Task<Scientist> CreateScientist(Scientist scientist);
        Task<Scientist> UpdateScientist(Scientist scientist);
        Task<Scientist> DeleteScientist(Scientist scientist);
        Task<Scientist> GetScientistById(Guid id);
    }
}
