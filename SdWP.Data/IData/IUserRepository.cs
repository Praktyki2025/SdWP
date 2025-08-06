using SdWP.Data.Models;
using SdWP.DTO.Responses;
using SdWP.DTO.Requests.Datatable;
using SdWP.Data.Repositories;

namespace SdWP.Data.IData
{
    internal interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameAvailableAsync(string username, string password);

        Task<UserListResponseDTO> FiltredAsync(DataTableRequestDTO request, UserRepository userRole, Guid userId);
    }
}
