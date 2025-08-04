using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IUserService
    {
        Task<ResultService<AddUserResponseDTO>> RegisterAsync(AddUserRequestDTO dto);
        Task<ResultService<List<UserListResponseDTO>>> GetUserListAsync();
        Task<ResultService<UserListResponseDTO>> DeleteUserAsync(DeleteUserRequestDTO dto);
        Task<ResultService<EditUserRequestDTO>> EditUserAsync(EditUserRequestDTO dto);
    }
}
