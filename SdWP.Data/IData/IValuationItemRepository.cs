using SdWP.Data.Models;

namespace SdWP.Data.IData
{
    public interface IValuationItemRepository
    {
        //jakies metody (definicje metod)
        Task<List<ValuationItem>> GetValuationItemsAsync();

    }
}
