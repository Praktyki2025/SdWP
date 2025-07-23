using SdWP.DTO.Requests;
using SdWP.DTO.Responses;

namespace SdWP.Service.IServices
{
    public interface IUserLoginService
    {
        Task<UserLoginResponseDTO?> LoginAsync(UserLoginRequestDTO dto);
    }
}
