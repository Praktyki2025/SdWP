using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public interface IValuationItemRepository
    {
        Task<ValuationItem> AddValuationItemAsync(ValuationItem valuationItem);
        Task<ValuationItem> UpdateValuationItemAsync(ValuationItem valuationItem);
        Task DeleteValuationItemAsync(Guid id);
        Task<ValuationItem?> GetValuationItemByIdAsync(Guid id);
        Task<List<ValuationItem>> GetAllValuationItemsAsync();
        Task<List<ValuationItem>> GetValuationItemsByValuationIdAsync(Guid valuationId); // Dodana metoda
    }
}