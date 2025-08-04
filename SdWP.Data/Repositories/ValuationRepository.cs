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
    public class ValuationRepository : IValuationRepository
    {
        private readonly ApplicationDbContext _context;

        public ValuationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Valuation> AddValuationAsync(Valuation valuation)
        {
            _context.Valuations.Add(valuation);
            await _context.SaveChangesAsync();
            return valuation;
        }

        public async Task<Valuation> UpdateValuationAsync(Valuation valuation)
        {
            _context.Valuations.Update(valuation);
            await _context.SaveChangesAsync();
            return valuation;
        }

        public async Task DeleteValuationAsync(Guid id)
        {
            var valuationToDelete = await _context.Valuations.FindAsync(id);
            if (valuationToDelete != null)
            {
                _context.Valuations.Remove(valuationToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Valuation?> GetValuationByIdAsync(Guid id)
        {
            return await _context.Valuations.FindAsync(id);
        }

        public async Task<List<Valuation>> GetAllValuationsAsync()
        {
            return await _context.Valuations.ToListAsync();
        }

        public async Task<List<Valuation>> GetValuationsByProjectIdAsync(Guid projectId)
        {
            return await _context.Valuations.Where(v => v.ProjectId == projectId).ToListAsync();
        }
    }
}