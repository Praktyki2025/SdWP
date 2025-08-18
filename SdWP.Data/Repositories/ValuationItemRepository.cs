using Microsoft.EntityFrameworkCore;
using SdWP.Data.Context;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;

namespace SdWP.Data.Repositories
{
    public class ValuationItemRepository : IValuationItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ValuationItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ValuationItem> AddValuationItemAsync(CreateValuationItemResponse response)
        {
            var valuationItem = new ValuationItem
            {
                Id = Guid.NewGuid(),
                Name = response.Name,
                ValuationId = response.ValuationId,
                Description = response.Description,
                CostTypeId = response.CostTypeId,
                UserGroupTypeId = response.UserGroupTypeId,
                Quantity = response.Quantity ?? throw new ArgumentNullException(nameof(response.Quantity)),
                UnitPrice = response.UnitPrice ?? throw new ArgumentNullException(nameof(response.UnitPrice)),
                TotalAmount = response.TotalAmount ?? throw new ArgumentNullException(nameof(response.TotalAmount)),
                RecurrencePeriod = response.RecurrencePeriod ?? throw new ArgumentNullException(nameof(response.RecurrencePeriod)),
                RecurrenceUnit = response.RecurrenceUnit,
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                CreatorUserId = response.CreatorUserId,
                CostCategoryID = response.CostCategoryID,
            };

            _context.ValuationItems.Add(valuationItem);
            
            await _context.SaveChangesAsync();
            return valuationItem;
        }

        public async Task<ValuationItem> UpdateValuationItemAsync(UpdateValuationItemResponse response)
        {
            var valuationItem = await _context.ValuationItems.FirstOrDefaultAsync(vi => vi.Id == response.Id);
            if (valuationItem == null) throw new KeyNotFoundException($"ValuationItem with ID {response.Id} not found.");

            valuationItem.Name = response.Name ?? valuationItem.Name;
            valuationItem.Description = response.Description ?? valuationItem.Description;
            valuationItem.LastModified = DateTime.UtcNow;
            valuationItem.CostTypeId = response.CostTypeId ?? valuationItem.CostTypeId;
            valuationItem.CostCategoryID = response.CostCategoryID ?? valuationItem.CostCategoryID;
            valuationItem.UserGroupTypeId = response.UserGroupTypeId ?? valuationItem.UserGroupTypeId;
            valuationItem.Quantity = response.Quantity ?? valuationItem.Quantity;
            valuationItem.UnitPrice = response.UnitPrice ?? valuationItem.UnitPrice;
            valuationItem.TotalAmount = response.TotalAmount ?? valuationItem.TotalAmount;
            valuationItem.RecurrencePeriod = response.RecurrencePeriod ?? valuationItem.RecurrencePeriod;
            valuationItem.RecurrenceUnit = response.RecurrenceUnit ?? valuationItem.RecurrenceUnit;

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


        public async Task<ValuationItem?> GetValuationItemByIdAsync(Guid id) =>
            await _context.ValuationItems.FindAsync(id);
        public async Task<List<ValuationItem>> GetAllValuationItemsAsync() =>
            await _context.ValuationItems.ToListAsync();

        public async Task<List<ValuationItem>> GetValuationItemsByValuationIdAsync(Guid valuationId) =>
              await _context.ValuationItems.Where(vi => vi.ValuationId == valuationId).ToListAsync();
    }
}