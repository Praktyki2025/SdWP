using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserLoginService _userLoginService;
        public LoginController(SignInManager<User> signInManager, IUserLoginService userLoginService)
        {
            _signInManager = signInManager;
            _userLoginService = userLoginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO dto)
        {
            var result = await _userLoginService.HandleLoginAsync(dto);

            if (result.Success) return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _userLoginService.HandleLogoutAsync();
            if (result.Success) return StatusCode(result.StatusCode, result.Data);

            return StatusCode(result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }
    }
}