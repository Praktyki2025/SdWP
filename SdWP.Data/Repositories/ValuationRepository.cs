using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;

namespace SdWP.Data.Repositories
{
    public class ValuationRepository : IValuationRepository
    {
        private readonly ApplicationDbContext _context;

        public ValuationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Valuation> AddValuationAsync(CreateValuationResponse request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var valuation = new Valuation
                {
                    Id = request.ValuationId,
                    Name = request.Name,
                    Description = request.Description,
                    CreatorUserId = request.CreatorUserId,
                    ProjectId = Guid.Empty,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    ValuationItems = new List<ValuationItem>()
                };

                _context.Valuations.Add(valuation);
                await _context.SaveChangesAsync();

                var valuationItem = new ValuationItem
                {
                    Id = Guid.NewGuid(),
                    ValuationId = valuation.Id,
                    CostTypeId = request.CostTypeId ?? throw new ArgumentNullException(nameof(request.CostTypeId)),
                    UserGroupTypeId = request.UserGroupTypeId ?? throw new ArgumentNullException(nameof(request.UserGroupTypeId)),
                    Quantity = request.Quantity ?? 1,
                    UnitPrice = request.UnitPrice ?? 0,
                    TotalAmount = request.TotalAmount ?? 0,
                    RecurrencePeriod = request.RecurrencePeriod ?? 0,
                    RecurrenceUnit = request.RecurrenceUnit,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow
                };

                _context.ValuationItems.Add(valuationItem);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return valuation;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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