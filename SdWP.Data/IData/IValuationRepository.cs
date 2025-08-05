using SdWP.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public interface IValuationRepository
    {
        Task<Valuation> AddValuationAsync(Valuation valuation);
        Task<Valuation> UpdateValuationAsync(Valuation valuation);
        Task DeleteValuationAsync(Guid id);
        Task<Valuation?> GetValuationByIdAsync(Guid id);
        Task<List<Valuation>> GetAllValuationsAsync();
        Task<List<Valuation>> GetValuationsByProjectIdAsync(Guid projectId);
    }
}
