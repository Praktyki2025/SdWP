using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/admin/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync([FromBody] AddUserRequestDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetUserListAsync([FromBody] DataTableRequestDTO request)
        {
            var result = await _userService.GetUserListAsync(request);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUserAsync(Guid id)
        {
            var dto = new DeleteUserRequestDTO { Id = id };
            var result = await _userService.DeleteUserAsync(dto);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpPut("update")]
        public async Task<IActionResult> EditUserAsync([FromBody] EditUserRequestDTO dto)
        {
            var result = await _userService.EditUserAsync(dto);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }
    }
}
