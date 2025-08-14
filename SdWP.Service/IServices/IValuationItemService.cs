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
        Task<ResultService<List<ValuationItemResponse>>> GetValuationList();
        Task<ResultService<DeleteValuationItemResponse>> DeleteValuationItem(Guid id);
        Task<ResultService<CreateValuationItemResponse>> CreateValuationItem(CreateValuationItemRequest request);
        Task<ResultService<UpdateValuationItemResponse>> UpdateValuationItem(UpdateValuationItemRequest request);
    }
}