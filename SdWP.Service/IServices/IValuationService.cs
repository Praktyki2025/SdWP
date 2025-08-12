using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Requests.Valuation.Name;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IValuationService
    {
        Task<ResultService<List<ValuationResponse>>> GetValuationList();
        Task<ResultService<CreateValuationResponse>> CreateValuation(CreateValuationRequest request);
        Task<ResultService<UserGroupNameRequest>> GetUserGroupName();
        Task<ResultService<CostTypeNameRequest>> GetCostTypeName();
        Task<ResultService<CostCategoryNameRequest>> GetCostCategoryName();
        Task<ResultService<ValuationDeleteResponse>> DeleteValuation(Guid id);
    }
}
