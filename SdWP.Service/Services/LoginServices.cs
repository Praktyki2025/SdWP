using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;

namespace SdWP.Service.Services
{
    public class LoginServices : ILoginService
    {
        private readonly IUserRepository _userRepository;
        public LoginServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResultService<LoginResponse>> HandleLoginAsync(LoginRequest dto)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    return ResultService<LoginResponse>.BadResult(
                        "Invalid email or password",
                        StatusCodes.Status401Unauthorized
                    );
                }

                var result = await _userRepository.PasswordSignInAsync(user, dto.Password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userRepository.GetRolesAsync(user);

                    return ResultService<LoginResponse>.GoodResult(
                        "Login successful",
                        StatusCodes.Status200OK,
                        new LoginResponse
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

                return ResultService<LoginResponse>.BadResult(
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                );
            }
            catch (Exception ex)
            {
                return ResultService<LoginResponse>.BadResult(
                    $"An error occurred during login: {ex.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<ResultService<string>> HandleLogoutAsync()
        {
            try
            {
                await _userRepository.SignOutAsync();

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
