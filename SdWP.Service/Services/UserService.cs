using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System.Security.Claims;

namespace SdWP.Service.Services
{
    public class UserService : IUserService
    {
        // In this services class, we handle user registration, delete, and edit user data.

        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultService<User>> ChangePasswordAsync(ChangePasswordRequest dto)
        {
            try
            {
                var principal = _httpContextAccessor.HttpContext?.User;

                if (principal == null || !principal.Identity.IsAuthenticated)
                {
                    return ResultService<User>.BadResult(
                        statusCode: StatusCodes.Status401Unauthorized,
                        message: "User is not authenticated."
                        );
                }

                var user = await _userManager.GetUserAsync(principal);

                if (user == null)
                {
                    return ResultService<User>.BadResult(
                        statusCode: StatusCodes.Status404NotFound,
                        message: "User not found."
                        );
                }

                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.PreviousPassword);
                if (!isPasswordCorrect)
                {
                    return ResultService<User>.BadResult(
                        statusCode: StatusCodes.Status400BadRequest,
                        message: "Wrong previous password"
                        );
                }

                if (dto.NewPassword != dto.ConfirmPassword)
                {
                    return ResultService<User>.BadResult(
                         statusCode: StatusCodes.Status400BadRequest,
                         message: "Passwords do not match"
                         );
                }

                if (dto.PreviousPassword == dto.ConfirmPassword)
                {
                    return ResultService<User>.BadResult(
                         statusCode: StatusCodes.Status400BadRequest,
                         message: "Passwords are the same"
                         );
                }

                var result = await _userManager.ChangePasswordAsync(user, dto.PreviousPassword, dto.NewPassword);

                if (!result.Succeeded)
                {
                    return ResultService<User>.BadResult(
                    statusCode: StatusCodes.Status400BadRequest,
                    message: "Wrong previous password",
                    errors: result.Errors.Select(e => e.Description).ToList()
                    );
                }

                return ResultService<User>.GoodResult(
                    statusCode: StatusCodes.Status200OK,
                    message: "Password changed successfully."
                    );

            }
            catch (Exception e)
            {
                return ResultService<User>.BadResult(
                    message: $"An error occurred during changing password: {e.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

        }

        public async Task<ResultService<RegisterResponseDTO>> RegisterAsync(RegisterRequestDTO dto)
        {
            try
            {
                var exist = await _userManager.FindByEmailAsync(dto.Email);
                if (exist != null)
                {
                    return ResultService<RegisterResponseDTO>.BadResult(
                        "User with this email already exists",
                        StatusCodes.Status400BadRequest
                    );
                }

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    NormalizedEmail = dto.Email.Normalize(),
                    UserName = dto.Name,
                    NormalizedUserName = dto.Name.Normalize(),
                    CreatedAt = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResultService<RegisterResponseDTO>.BadResult(
                        "User creation failed",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                var createdUser = await _userManager.FindByEmailAsync(dto.Email);
                if (createdUser == null)
                {
                    return ResultService<RegisterResponseDTO>.BadResult(
                        "User was created but could not be loaded for role assignment",
                        StatusCodes.Status400BadRequest
                    );
                }

                var roleResult = await _userManager.AddToRoleAsync(createdUser, "User");
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    return ResultService<RegisterResponseDTO>.BadResult(
                        "User was created but failed to assign role to user",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                var roles = await _userManager.GetRolesAsync(createdUser);

                var responseDto = new RegisterResponseDTO
                {
                    Success = true,
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Name = createdUser.Name,
                    CreatedAt = createdUser.CreatedAt,
                    Message = "User registered successfully",
                    Roles = roles.ToList()
                };

                return ResultService<RegisterResponseDTO>.GoodResult(
                    "User registered successfully",
                    statusCode: StatusCodes.Status201Created,
                    responseDto
                );
            }
            catch (Exception e)
            {
                return ResultService<RegisterResponseDTO>.BadResult(
                    $"An error occurred during registration: {e.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
