using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;

namespace SdWP.Data.IData
{
    public interface IUserRepository
    {
        Task<UserListResponseDTO> FiltredAsync(DataTableRequestDTO request, Guid userId);
        Task<List<UserListResponseDTO>> GetUserAsync(DataTableRequestDTO request);
        Task<User?> FindByIdAsync(string userId);
    }
}
