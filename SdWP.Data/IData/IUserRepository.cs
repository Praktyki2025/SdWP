using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<UserListResponse> FiltredAsync(DataTableRequest request, Guid userId);
        Task<List<UserListResponse>> GetUserAsync(DataTableRequest request);
        Task<User?> FindByIdAsync(string userId);
    }
}
