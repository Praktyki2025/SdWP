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
        public async Task<ActionResult> RegisterAsync([FromBody] AddUserRequest dto)
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
        public async Task<IActionResult> GetUserListAsync([FromBody] DataTableRequest request)
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
            var dto = new DeleteUserRequest { Id = id };
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
        public async Task<IActionResult> EditUserAsync([FromBody] EditUserRequest dto)
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
