using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IValuationItemService
    {
        Task<ResultService<IEnumerable<ValuationItemResponse>>> GetAllValuationItemsAsync();
        Task<ResultService<ValuationItemResponse>> GetValuationItemByIdAsync(Guid id);
        Task<ResultService<IEnumerable<ValuationItemResponse>>> GetValuationItemsByValuationIdAsync(Guid valuationId);
        Task<ResultService<ValuationItemResponse>> CreateValuationItemAsync(CreateValuationItemRequest request);
        Task<ResultService<ValuationItemResponse>> UpdateValuationItemAsync(UpdateValuationItemRequest request);
        Task<ResultService<string>> DeleteValuationItemAsync(Guid id);
    }
}