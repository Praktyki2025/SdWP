using SdWP.DTO.Responses;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IValuationService
    {
        Task<ResultService<List<ValuationResponse>>> GetValuationList();
    }
}