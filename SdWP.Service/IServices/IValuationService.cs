using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IValuationService
    {
        Task<ResultService<IEnumerable<ValuationResponse>>> GetAllValuationsAsync();
        Task<ResultService<ValuationResponse>> GetValuationByIdAsync(Guid id);
        Task<ResultService<IEnumerable<ValuationResponse>>> GetValuationsByProjectIdAsync(Guid projectId);
        Task<ResultService<ValuationResponse>> CreateValuationAsync(CreateValuationRequest request);
        Task<ResultService<ValuationResponse>> UpdateValuationAsync(UpdateValuationRequest request);
        Task<ResultService<string>> DeleteValuationAsync(Guid id);
    }
}