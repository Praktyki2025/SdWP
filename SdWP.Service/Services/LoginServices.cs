using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using Microsoft.AspNetCore.Http;

namespace SdWP.Service.Services
{
    public class LoginServices : ILoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public LoginServices(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ResultService<LoginResponseDTO>> HandleLoginAsync(LoginRequestDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ResultService<LoginResponseDTO>.BadResult(
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    );
                }

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    return ResultService<LoginResponseDTO>.GoodResult(
                        "Login successful",
                        StatusCodes.Status200OK,
                        new LoginResponseDTO
                        {
                            Success = true,
                            Id = user.Id,
                            Email = user.Email!,
                            Name = user.Name,
                            LoginTime = DateTime.UtcNow,
                            Roles = roles.ToList()
                        }
                    );
                }

                return ResultService<LoginResponseDTO>.BadResult(
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                );
            }
            catch (Exception ex)
            {
                return ResultService<LoginResponseDTO>.BadResult(
                    $"An error occurred during login: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<ResultService<string>> HandleLogoutAsync()
        {
            try
            {
                await _signInManager.SignOutAsync();

                return ResultService<string>.GoodResult(
                    "Logout successful",
                    StatusCodes.Status200OK
                );
            }
            catch (Exception e)
            {
                return ResultService<string>.BadResult(
                    $"An error occurred during logout: {e.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
