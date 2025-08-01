using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
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
        public async Task<ActionResult> RegisterAsync(RegisterRequestDTO dto)
        {
            var result = await _userService.RegisterAsync(dto);

            if (result.Success) return StatusCode( result.StatusCode, result.Data);

            return StatusCode( result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetUserListAsync()
        {
            var result = await _userService.GetUserListAsync();

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
