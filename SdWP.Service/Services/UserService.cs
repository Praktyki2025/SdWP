using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class UserService : IUserService
    {
        // In this services class, we handle user registration, delete, and edit user data.

        private readonly UserManager<User> _userManager;
        private readonly UserRepository _userRepository;

        public UserService(UserManager<User> userManager, UserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
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

        public async Task<ResultService<List<UserListResponseDTO>>> GetUserListAsync()
        {
            try
            {
                var users = await _userRepository.GetUserRoleAsync(CancellationToken.None);

                if (users == null)
                {
                    return ResultService<List<UserListResponseDTO>>.BadResult(
                        "No users found",
                        StatusCodes.Status404NotFound
                    );
                }

                var userList = users.Select(u => new UserListResponseDTO
                {
                    Id = u.user.Id,
                    Email = u.user.Email,
                    Name = u.user.Name,
                    Roles = u.Roles ?? new List<string>(),
                    CreatedAt = u.user.CreatedAt,
                    Success = true
                }).ToList();

                return ResultService<List<UserListResponseDTO>>.GoodResult(
                        "User get successfull",
                        StatusCodes.Status200OK,
                        userList
                );
            }
            catch (Exception e)
            {
                return ResultService<List<UserListResponseDTO>>.BadResult(
                    $"An error occurred while retrieving users: {e.Message}",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                );
            }
        }

        public async Task<ResultService<UserListResponseDTO>> DeleteUserAsync(UserDeleteRequestDTO dto)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(dto.Id.ToString(), CancellationToken.None);
                if (user == null)
                {
                    return ResultService<UserListResponseDTO>.BadResult(
                        "User not found",
                        StatusCodes.Status404NotFound
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Contains("Admin"))
                {
                    return ResultService<UserListResponseDTO>.BadResult(
                        "Cannot delete an admin user",
                        StatusCodes.Status403Forbidden
                    );
                }

                var responseDto = new UserListResponseDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Roles = roles?.ToList() ?? new List<string>(),
                    CreatedAt = user.CreatedAt,
                    Success = true
                };

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return ResultService<UserListResponseDTO>.GoodResult(
                        "User deleted successfully",
                        StatusCodes.Status200OK,
                        responseDto
                    );
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResultService<UserListResponseDTO>.BadResult(
                        "User deletion failed",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }
            }
            catch (Exception e)
            {
                return ResultService<UserListResponseDTO>.BadResult(
                    $"An error occurred while deleting the user: {e.Message}",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                );
            }
        }
    }
}
