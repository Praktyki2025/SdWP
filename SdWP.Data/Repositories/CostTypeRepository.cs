using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;

namespace SdWP.Data.Repositories
{
    public class CostTypeRepository : ICostTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public CostTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetGuidByName(string name)
        {
            var costType = await _context.CostTypes
                .Where(ct => ct.Name == name)
                .Select(ct => ct.Id)
                .FirstOrDefaultAsync();

            if (costType == Guid.Empty) throw new KeyNotFoundException($"Cost type with name '{name}' not found.");

            return costType;
        }

        public Task<List<string>> GetAllNamesAsync()
            => _context.CostTypes.Select(ct => ct.Name).ToListAsync();
    }
}
