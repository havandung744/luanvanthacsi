using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface ISecretaryService
    {
        Task<List<Secretary>> GetAllByIdAsync(string id);
    }
}
