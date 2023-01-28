using luanvanthacsi.Api.Data;
using luanvanthacsi.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace luanvanthacsi.Api.Repositores
{
    public class ScientistRepository : IScientistRepository
    {
        private readonly luanvanthacsiDbContext _context;
        public ScientistRepository(luanvanthacsiDbContext context)
        {
            _context = context;
        }

        public Task<Scientist> CreateScientist(Scientist scientist)
        {
            throw new NotImplementedException();
        }

        public Task<Scientist> DeleteScientist(Scientist scientist)
        {
            throw new NotImplementedException();
        }

        public Task<Scientist> GetScientistById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Scientist>> GetScientistsAsync()
        {
            return await _context.scientists.ToListAsync();
        }

        public Task<Scientist> UpdateScientist(Scientist scientist)
        {
            throw new NotImplementedException();
        }
    }
}
