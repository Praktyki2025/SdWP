using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<UserListResponseDTO> FiltredAsync(DataTableRequestDTO request, Guid userId);
        Task<List<(User user, List<string> Roles)>> GetUserAsync(DataTableRequestDTO request, CancellationToken cancellationToken);
        Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken);
    }
}
