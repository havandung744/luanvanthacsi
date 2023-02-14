using luanvanthacsi.Data.Entities;

namespace luanvanthacsi.Data.Services
{
    public interface IThesisDefenseService
    {
        Task<List<ThesisDefense>> GetAllAsync();
        Task<bool> AddOrUpdateThesisDefenseAsync(ThesisDefense thesisDefense);
        Task<ThesisDefense> GetThesisDefenseByIdAsync(string id);
        Task<bool> DeleteThesisDefenseAsync(ThesisDefense thesisDefense);
        Task<bool> DeleteThesisDefenseListAsync(List<ThesisDefense> thesisDefenses);
        Task<List<ThesisDefense>> GetListThesisDefenseBySearchAsync(string txtSearch);
        Task<List<Student>> GetCurrentListStaff(string FacultyId, string thesisDefensesId);

    }
}
