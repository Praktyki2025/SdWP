using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface ILinkServices
    {
        Task<ResultService<LinkResponse>> AddLinkAsync(AddLinkRequest request);
        Task<ResultService<List<LinkResponse>>> GetLinkByProjectId(Guid projectId);
        Task<ResultService<bool>> DeleteLinkAsync(Guid id);
        Task<ResultService<UpdateLinkResponse>> UpdateLinkAsync(UpdateLinkRequest request);
    }
}
