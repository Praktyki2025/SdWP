using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;

namespace SdWP.Service.IServices
{
    public interface IUserLoginService
    {
        Task<User?> ValidateUserAsync(UserLoginRequestDTO dto);
        Task<IList<string>> GetUserRolesAsync(User user);
    }
}
