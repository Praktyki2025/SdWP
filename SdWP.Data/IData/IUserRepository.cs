using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<UserListResponse> FiltredAsync(DataTableRequest request, Guid userId);
        Task<List<UserListResponse>> GetUsersAsync(DataTableRequest request);
        Task<User?> FindByIdAsync(string userId);
    }
}
