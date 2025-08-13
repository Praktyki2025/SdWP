using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;


namespace SdWP.Data.Repositories
{
    public class CostCategoryRepsoitory : ICostCategoryRepsoitory
    {
        private readonly ApplicationDbContext _context;

        public CostCategoryRepsoitory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> GetGuidByName(string name)
        {
            var costCategory = await _context.CostCategories
                .Where(cc => cc.Name == name)
                .Select(cc => cc.Id)
                .FirstOrDefaultAsync();
            if (costCategory == Guid.Empty) throw new KeyNotFoundException($"Cost category with name '{name}' not found.");
            return costCategory;
        }

        public Task<List<string>> GetAllNamesAsync()
            => _context.CostCategories.Select(cc => cc.Name).ToListAsync();
    }
}
