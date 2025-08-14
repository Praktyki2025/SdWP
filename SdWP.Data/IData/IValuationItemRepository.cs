using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;
namespace SdWP.Data.IData
{
    public interface IValuationItemRepository
    {
        Task<ValuationItem> AddValuationItemAsync(CreateValuationItemResponse response);
        Task<ValuationItem> UpdateValuationItemAsync(UpdateValuationItemResponse response);
        Task DeleteValuationItemAsync(Guid id);
        Task<ValuationItem?> GetValuationItemByIdAsync(Guid id);
        Task<List<ValuationItem>> GetAllValuationItemsAsync();
        Task<List<ValuationItem>> GetValuationItemsByValuationIdAsync(Guid valuationId);
    }
}