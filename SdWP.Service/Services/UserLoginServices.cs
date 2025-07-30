using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System.Data;

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

        public async Task<User?> ValidateUserAsync(UserLoginRequestDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            return isValid ? user : null;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<ResultService<UserLoginResponseDTO>> HandleLoginAsync(UserLoginRequestDTO dto)
        {
            try
            {
                Console.WriteLine($"Login attempt for: {dto.Email}");

                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    Console.WriteLine($"User not found: {dto.Email}");
                    return ResultService<UserLoginResponseDTO>.BadResult(
                        null,
                        "Invalid email or password",
                        401
                    );
                }

                Console.WriteLine($"User found: {user.Email}, ID: {user.Id}");

                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    Console.WriteLine("SignInManager sign-in successful.");
                    var roles = await _userManager.GetRolesAsync(user);
                    Console.WriteLine($"User roles: {string.Join(", ", roles)}");

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
                        200
                    );
                }

                Console.WriteLine("SignInManager sign-in failed.");
                return ResultService<UserLoginResponseDTO>.BadResult(
                    null,
                    "Invalid email or password",
                    401
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception: {ex}");

                return ResultService<UserLoginResponseDTO>.BadResult(
                    null,
                    $"An error occurred during login: {ex.Message}",
                    500
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
                    200
                );
            }
            catch (Exception e)
            {
                return ResultService<string>.BadResult(
                    null,
                    $"An error occurred during logout: {e.Message}",
                    500
                );
            }
        }
    }
}
