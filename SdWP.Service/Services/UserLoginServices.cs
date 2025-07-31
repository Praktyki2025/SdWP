using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using Microsoft.AspNetCore.Http;

namespace SdWP.Service.Services
{
    public class UserLoginServices : IUserLoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UserLoginServices(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ResultService<UserLoginResponseDTO>> HandleLoginAsync(UserLoginRequestDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ResultService<UserLoginResponseDTO>.BadResult(
                        null,
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    );
                }

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    return ResultService<UserLoginResponseDTO>.GoodResult(
                        new UserLoginResponseDTO
                        {
                            Success = true,
                            Id = user.Id,
                            Email = user.Email!,
                            Name = user.Name,
                            LoginTime = DateTime.UtcNow,
                            Roles = roles.ToList()
                        },
                        "Login successful",
                        StatusCodes.Status200OK
                    );
                }

                return ResultService<UserLoginResponseDTO>.BadResult(
                    null,
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                );
            }
            catch (Exception ex)
            {
                return ResultService<UserLoginResponseDTO>.BadResult(
                    null,
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
                    null,
                    "Logout successful",
                    StatusCodes.Status200OK
                );
            }
            catch (Exception e)
            {
                return ResultService<string>.BadResult(
                    null,
                    $"An error occurred during logout: {e.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
