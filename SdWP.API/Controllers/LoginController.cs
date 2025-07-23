using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using Microsoft.AspNetCore.Antiforgery;

namespace SdWP.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IAntiforgery _antiforgery;

        public LoginController(SignInManager<User> signInManager, UserManager<User> userManager, IAntiforgery antiforgery)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _antiforgery = antiforgery;
        }



        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO loginDto)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);

                if (!ModelState.IsValid)
                {
                    return BadRequest(new UserLoginResponseDTO
                    {
                        Success = false,
                        Message = "Invalid input"
                    });
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    return StatusCode(401, new UserLoginResponseDTO
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    return StatusCode(401, new UserLoginResponseDTO
                    {
                        Success = false,
                        Message = $"Invalid email or password"
                    });
                }

                return Ok(new UserLoginResponseDTO
                {
                    Success = true,
                    Id = user.Id,
                    Email = user.Email!,
                    Name = user.Name,
                    LoginTime = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new UserLoginResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                });
            }
        }

    }
}
