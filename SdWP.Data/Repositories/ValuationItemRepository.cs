using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;

namespace SdWP.Data.Repositories
{
    public class ValuationItemRepository : IValuationItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ValuationItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ValuationItem> AddValuationItemAsync(ValuationItem valuationItem)
        {
            _context.ValuationItems.Add(valuationItem);
            await _context.SaveChangesAsync();
            return valuationItem;
        }

        public async Task<ValuationItem> UpdateValuationItemAsync(ValuationItem valuationItem)
        {
            _context.ValuationItems.Update(valuationItem);
            await _context.SaveChangesAsync();
            return valuationItem;
        }

        public async Task DeleteValuationItemAsync(Guid id)
        {
            var valuationItemToDelete = await _context.ValuationItems.FindAsync(id);
            if (valuationItemToDelete != null)
            {
                _context.ValuationItems.Remove(valuationItemToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ValuationItem?> GetValuationItemByIdAsync(Guid id)
        {
            return await _context.ValuationItems.FindAsync(id);
        }

        public async Task<List<ValuationItem>> GetAllValuationItemsAsync()
        {
            return await _context.ValuationItems.ToListAsync();
        }

        public async Task<List<ValuationItem>> GetValuationItemsByValuationIdAsync(Guid valuationId)
        {
            return await _context.ValuationItems.Where(vi => vi.ValuationId == valuationId).ToListAsync();
        }
    }
}