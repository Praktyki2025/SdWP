using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
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
        private readonly IUserRepository _userRepository;
        private readonly IErrorLogHelper _errorLogServices;
        private string message = string.Empty;

        public LoginServices(
            UserRepository userRepository,
            IErrorLogHelper errorLogServices)
        {
            _userRepository = userRepository;
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<LoginResponse>> HandleLoginAsync(LoginRequest dto)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(dto.Email);
                if (user == null)
                {
                    message = $"Login attempt with invalid email: {dto.Email}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.HandleLoginAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LoginResponse>.BadResult(
                            message,
                            StatusCodes.Status401Unauthorized
                        ));
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
                else
                {
                    message = $"Login failed for user: {dto.Email}. Result: {result.ToString()}";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.HandleLoginAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO);
                }

                return ResultService<LoginResponse>.BadResult(
                    "Invalid email or password",
                    StatusCodes.Status401Unauthorized
                );
            }
            catch (Exception e)
            {
                message = $"Error: {e.Message}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<LoginResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                    ));
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
                message = $"Error during logout: {e.Message}";
                Log.Error(message);
                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.HandleLogoutAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
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
