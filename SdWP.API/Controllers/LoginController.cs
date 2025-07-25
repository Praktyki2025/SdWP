using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using System.Security.Claims;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor; 

        public LoginController(SignInManager<User> signInManager,
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDTO loginDto)
        {
            try
            {
                Console.WriteLine($"Login attempt for: {loginDto.Email}");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new UserLoginResponseDTO { Success = false, Message = "Invalid input" });
                }

                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {loginDto.Email}");
                    return StatusCode(401, new UserLoginResponseDTO { Success = false, Message = "Invalid email or password" });
                }

                Console.WriteLine($"User found: {user.Email}, ID: {user.Id}");

                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    Console.WriteLine("SignInManager sign-in successful.");
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine($"User roles: {string.Join(", ", roles)}");

                    return Ok(new UserLoginResponseDTO
                    {
                        Success = true,
                        Id = user.Id,
                        Email = user.Email!,
                        Name = user.Name,
                        LoginTime = DateTime.UtcNow,
                        Roles = roles.ToList(),
                    });
                }

                Console.WriteLine("SignInManager sign-in failed.");
                return StatusCode(401, new UserLoginResponseDTO { Success = false, Message = "Invalid email or password" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex}");
                return StatusCode(500, new UserLoginResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred during login: {ex.Message}"
                });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Success = true, Message = "Logged out successfully" });
        }
    }
}