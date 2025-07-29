using SdWP.Data.Models;

namespace SdWP.Data.Interfaces
{
    public interface IValuationItemRepository
    {
        //jakies metody (definicje metod)
        Task<List<ValuationItem>> GetValuationItemsAsync();

    }
}
