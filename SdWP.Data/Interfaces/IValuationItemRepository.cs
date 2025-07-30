using SdWP.Data.Models;

namespace SdWP.Data.Interfaces
{
    public interface IValuationItemRepository
    {
        Task <ValuationItem> GetValuationItemByIdAsync(Guid id);

        //jakies metody (definicje metod)
        Task<List<ValuationItem>> GetValuationItemsAsync();
        Task<bool> DeleteValuationItemAsync(Guid id);
    }
}
