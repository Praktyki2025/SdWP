using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;
using System.Security.Claims;


namespace SdWP.Service.IServices
{
    public interface IUserService
    {
        Task<ResultService<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO dto);
        Task<ResultService<User>> GetCurrentUser(ClaimsPrincipal userPrincipal);
        Task<ResultService<User>> ChangePasswordAsync(User user, ChangePasswordRequest dto);
    }
}
