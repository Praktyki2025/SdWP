using SdWP.DTO.Requests;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Requests.Mailing;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.DataTable;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IUserService
    {
        Task<ResultService<AddUserResponse>> RegisterAsync(AddUserRequest dto);
        Task<ResultService<DataTableResponse<UserListResponse>>> GetUserListAsync(DataTableRequest request);
        Task<ResultService<UserListResponse>> DeleteUserAsync(DeleteUserRequest dto);
        Task<ResultService<EditUserRequest>> EditUserAsync(EditUserRequest dto);
        Task<ResultService<string>> ResetPasswordAsync(ResetPasswordRequest dto);
    }
}
