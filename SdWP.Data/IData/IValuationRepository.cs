using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Data.IData
{
    public interface IValuationRepository
    {
        Task<Valuation> AddValuationAsync(CreateValuationResponse request);
        Task<Valuation> UpdateValuationAsync(UpdateValuationResponse request);
        Task DeleteValuationAsync(Guid id);
        Task<Valuation?> GetValuationByIdAsync(Guid id);
        Task<List<Valuation>> GetAllValuationsAsync();
        Task<List<Valuation>> GetValuationsByProjectIdAsync(Guid projectId);
    }
}
