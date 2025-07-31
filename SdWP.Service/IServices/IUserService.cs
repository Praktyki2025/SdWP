using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IUserService
    {
        Task<ResultService<UserRegisterResponseDTO>> RegisterAsync(UserRegisterRequestDTO dto);
    }
}
