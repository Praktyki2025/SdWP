using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.Interfaces;
using SdWP.Data.Models;


namespace SdWP.Data.Repositories
{
    public class ValuationItemRepository : IValuationItemRepository
    {


        private readonly ApplicationDbContext _context; 
        public ValuationItemRepository(ApplicationDbContext context)
        {
            _context = context; //_context to laczenie z baza 
        }




        public async Task<List<ValuationItem>> GetValuationItemsAsync()
        {
           var valuationItems = await _context.ValuationItems.ToListAsync();
            return valuationItems;
        }
    }
}
