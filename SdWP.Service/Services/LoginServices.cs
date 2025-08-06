using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;

namespace SdWP.Service.Services
{
    public class LoginServices : ILoginService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IErrorLogHelper _errorLogServices;
        private string message = string.Empty;

        public LoginServices(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IErrorLogHelper errorLogServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<LoginResponseDTO>> HandleLoginAsync(LoginRequestDTO dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    message = $"Login attempt with invalid email: {dto.Email}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.HandleLoginAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LoginResponseDTO>.BadResult(
                            message,
                            StatusCodes.Status401Unauthorized
                        ));
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
                else
                {
                    message = $"Login failed for user: {dto.Email}. Result: {result.ToString()}";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.HandleLoginAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO);
                }

                return ResultService<LoginResponseDTO>.BadResult(
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                );
            }
            catch (Exception e)
            {
                message = $"Error: {e.Message}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.HandleLoginAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith (_ => ResultService<LoginResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                    ));
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
                message = $"Error during logout: {e.Message}";
                Log.Error(message);
                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.HandleLogoutAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<string>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                    ));
            }
        }
    }
}
