using SdWP.Data.Models;
using SdWP.DTO.Responses.Valuation;

namespace SdWP.Data.IData
{
    public interface ILinkRepository
    {
        Task<Link> AddLinkAsync(LinkResponse response);
        Task<List<Link>> GetAllLinksToProject(Guid id);
        Task<bool> DeleteLinkAsync(Guid id);
        Task<bool> FindLinkByIdAsync(Guid id);
        Task<Link> UpdateLinkAsync(UpdateLinkResponse response);
    }
}
