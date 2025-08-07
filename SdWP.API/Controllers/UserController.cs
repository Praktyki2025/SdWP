using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{


    [ApiController]
    [Route("api/user")]
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

        [HttpPost("newpassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        { 
            var user = await _userService.GetCurrentUser(User);
            var result = await _userService.ChangePasswordAsync(user.Data, model);

            if (result.Success) return StatusCode(result.StatusCode, result.Data);

            else
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }
        }
    }
}
