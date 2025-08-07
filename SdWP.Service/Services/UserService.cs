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

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ResultService<User>> ChangePasswordAsync(User user, ChangePasswordRequest dto)
        {
            if (user == null)
            {
                return new ResultService<User>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found.",
                };
            }

            await _userManager.ChangePasswordAsync(user, dto.PrevPassword, dto.NewPassword);

            return new ResultService<User>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Password changed successfully.",
            };
        }

        public async Task<ResultService<User>> GetCurrentUser(ClaimsPrincipal userPrincipal)
        {
            var emailClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            var email = emailClaim?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return new ResultService<User>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "User is not authenticated."
                };
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new ResultService<User>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "User not found."
                };
            }

            // Возвращаем полный объект пользователя, а не DTO, если нужен полный доступ
            return new ResultService<User>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = user,
                Message = "User retrieved successfully."
            };
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
