using SdWP.DTO.Requests;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.DataTable;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IUserService
    {
        Task<ResultService<AddUserResponseDTO>> RegisterAsync(AddUserRequestDTO dto);
        Task<ResultService<DataTableResponseDTO<UserListResponseDTO>>> GetUserListAsync(DataTableRequestDTO request);
        Task<ResultService<UserListResponseDTO>> DeleteUserAsync(DeleteUserRequestDTO dto);
        Task<ResultService<EditUserRequestDTO>> EditUserAsync(EditUserRequestDTO dto);
    }
}
