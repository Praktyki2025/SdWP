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

        public async Task<Valuation> AddValuationAsync(CreateValuationResponse response)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var valuation = new Valuation
                {
                    Id = response.ValuationId,
                    Name = response.Name,
                    Description = response.Description,
                    CreatorUserId = response.CreatorUserId,
                    ProjectId = response.ProjectId, 
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    ValuationItems = new List<ValuationItem>(),
                };

                _context.Valuations.Add(valuation);
                await _context.SaveChangesAsync();

                var valuationItem = new ValuationItem
                {
                    Id = Guid.NewGuid(),
                    Name = response.Name,
                    ValuationId = valuation.Id,
                    Description = response.Description,
                    CostTypeId = response.CostTypeId ?? throw new ArgumentNullException(nameof(response.CostTypeId)),
                    UserGroupTypeId = response.UserGroupTypeId ?? throw new ArgumentNullException(nameof(response.UserGroupTypeId)),
                    Quantity = response.Quantity ?? throw new ArgumentNullException(nameof(response.Quantity)),
                    UnitPrice = response.UnitPrice ?? throw new ArgumentNullException(nameof(response.UnitPrice)),
                    TotalAmount = response.TotalAmount ?? throw new ArgumentNullException(nameof(response.TotalAmount)),
                    RecurrencePeriod = response.RecurrencePeriod ?? throw new ArgumentNullException(nameof(response.RecurrencePeriod)),
                    RecurrenceUnit = response.RecurrenceUnit,
                    CreatedAt = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,
                    CreatorUserId = response.CreatorUserId,
                    CostCategoryID = response.CostCategoryID ?? throw new ArgumentNullException(nameof(response.CostCategoryID)),
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

        public async Task<Valuation> UpdateValuationAsync(UpdateValuationResponse response)
        {
            var valuation = await _context.Valuations.FirstOrDefaultAsync(v => v.Id == response.Id);
            if (valuation == null) throw new Exception("Valuation not found");

            valuation.Name = response.Name ?? throw new ArgumentNullException(nameof(response.Name));
            valuation.Description = response.Description ?? throw new ArgumentNullException(nameof(response.Description));
            valuation.LastModified = response.LastModified;
            valuation.ProjectId = response.ProjectId ?? throw new ArgumentNullException(nameof(response.ProjectId)); ;
            valuation.CreatorUserId = response.CreatorUserId;

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
            => await _context.Valuations.ToListAsync();

        public async Task<List<Valuation>> GetValuationsByProjectIdAsync(Guid projectId)
        {
            return await _context.Valuations.Where(v => v.ProjectId == projectId).ToListAsync();
        }
    }
}