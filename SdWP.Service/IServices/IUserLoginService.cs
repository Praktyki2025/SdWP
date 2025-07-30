using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IUserLoginService
    {
        Task<ResultService<UserLoginResponseDTO>> HandleLoginAsync(UserLoginRequestDTO dto);
        Task<ResultService<string>> HandleLogoutAsync();
    }
}
