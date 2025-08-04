using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;
using Microsoft.AspNetCore.Http;

namespace SdWP.Service.IServices
{
    public interface ILoginService
    {
        Task<ResultService<LoginResponseDTO>> HandleLoginAsync(LoginRequestDTO dto);
        Task<ResultService<string>> HandleLogoutAsync();
    }
}
