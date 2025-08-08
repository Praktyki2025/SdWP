using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
using SdWP.DTO.Requests;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.DataTable;
using SdWP.Service.IServices;
using SdWP.Data.IData;

namespace SdWP.Service.Services
{
    public class UserService : IUserService
    {
        // In this services class, we handle user registration, delete, and edit user data.

        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        private readonly List<string> _roles = new();

        public UserService(
            UserManager<User> userManager,
            IUserRepository userRepository,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
        }

        public async Task<ResultService<AddUserResponse>> RegisterAsync(AddUserRequest dto)
        {
            try
            {
                var exist = await _userManager.FindByEmailAsync(dto.Email);
                if (exist != null)
                {
                    return ResultService<AddUserResponse>.BadResult(
                        "User with this email already exists",
                        StatusCodes.Status400BadRequest
                    );
                }

                if (string.IsNullOrEmpty(dto.Role))
                {
                    return ResultService<AddUserResponse>.BadResult(
                        "Invalid role specified",
                        StatusCodes.Status400BadRequest
                    );
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    return ResultService<AddUserResponse>.BadResult(
                        "Role does not exist",
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
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResultService<AddUserResponse>.BadResult(
                        "User creation failed",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                var createdUser = await _userManager.FindByEmailAsync(dto.Email);
                if (createdUser == null)
                {
                    return ResultService<AddUserResponse>.BadResult(
                        "User was created but could not be loaded for role assignment",
                        StatusCodes.Status400BadRequest
                    );
                }

                var roleResult = await _userManager.AddToRoleAsync(createdUser, dto.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    return ResultService<AddUserResponse>.BadResult(
                        "User was created but failed to assign role to user",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                var roles = await _userManager.GetRolesAsync(createdUser);

                var responseDto = new AddUserResponse
                {
                    Success = true,
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Name = createdUser.Name,
                    CreatedAt = createdUser.CreatedAt,
                    Message = "User registered successfully",
                    Roles = roles.ToList()
                };

                return ResultService<AddUserResponse>.GoodResult(
                    "User registered successfully",
                    statusCode: StatusCodes.Status201Created,
                    responseDto
                );
            }
            catch (Exception e)
            {
                return ResultService<AddUserResponse>.BadResult(
                    $"An error occurred during registration: {e.Message}",
                    StatusCodes.Status500InternalServerError
                );
            }
        }

        public async Task<ResultService<DataTableResponse<UserListResponse>>> GetUserListAsync(DataTableRequest request)
        {
            try
            {
                var users = await _userRepository.GetUsersAsync(request);

                if (users == null || users.Count == 0)
                {
                    return ResultService<DataTableResponse<UserListResponse>>.BadResult(
                        "No users found",
                        StatusCodes.Status404NotFound
                    );
                }


                var dataTableResponse = new DataTableResponse<UserListResponse>
                {
                    Draw = request.Draw,
                    RecordsTotal = users.Count,
                    RecordsFiltered = users.Count,
                    Data = users
                };

                return ResultService<DataTableResponse<UserListResponse>>.GoodResult(
                    "Users retrieved successfully",
                    StatusCodes.Status200OK,
                    dataTableResponse
                );
            }
            catch (Exception e)
            {
                return ResultService<DataTableResponse<UserListResponse>>.BadResult(
                    $"An error occurred while retrieving users: {e.Message}",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                );
            }
        }

        public async Task<ResultService<UserListResponse>> DeleteUserAsync(DeleteUserRequest dto)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(dto.Id.ToString());
                if (user == null)
                {
                    return ResultService<UserListResponse>.BadResult(
                        "User not found",
                        StatusCodes.Status404NotFound
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Contains("Admin"))
                {
                    return ResultService<UserListResponse>.BadResult(
                        "Cannot delete an admin user",
                        StatusCodes.Status403Forbidden
                    );
                }

                var responseDto = new UserListResponse
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
                    return ResultService<UserListResponse>.GoodResult(
                        "User deleted successfully",
                        StatusCodes.Status200OK,
                        responseDto
                    );
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResultService<UserListResponse>.BadResult(
                        "User deletion failed",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }
            }
            catch (Exception e)
            {
                return ResultService<UserListResponse>.BadResult(
                    $"An error occurred while deleting the user: {e.Message}",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                );
            }
        }

        public async Task<ResultService<EditUserRequest>> EditUserAsync(EditUserRequest dto)
        {
            try
            {
                var exist = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (exist == null)
                {
                    return ResultService<EditUserRequest>.BadResult(
                        "User not found",
                        StatusCodes.Status400BadRequest
                    );
                }

                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != exist.Email)
                {
                    var userWithEmail = await _userManager.FindByEmailAsync(dto.Email);
                    if (userWithEmail != null && userWithEmail.Id != exist.Id)
                    {
                        return ResultService<EditUserRequest>.BadResult(
                            "User with this email already exists",
                            StatusCodes.Status400BadRequest
                        );
                    }
                }

                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != exist.Name)
                {
                    var userWithName = await _userManager.FindByNameAsync(dto.Name);
                    if (userWithName != null && userWithName.Id != exist.Id)
                    {
                        return ResultService<EditUserRequest>.BadResult(
                            "User with this name already exists",
                            StatusCodes.Status400BadRequest
                        );
                    }
                }

                if (!string.IsNullOrEmpty(dto.Role) && !await _roleManager.RoleExistsAsync(dto.Role))
                {
                    return ResultService<EditUserRequest>.BadResult(
                        "Role does not exist",
                        StatusCodes.Status400BadRequest
                    );
                }


                if (!string.IsNullOrEmpty(dto.Name))
                {
                    exist.Name = dto.Name;
                    exist.NormalizedUserName = dto.Name.Normalize();
                    exist.UserName = dto.Name;
                }

                if (!string.IsNullOrEmpty(dto.Email))
                {
                    exist.Email = dto.Email;
                    exist.NormalizedEmail = dto.Email.Normalize();
                }

                exist.LastUpdate = DateTime.UtcNow;

                if (!string.IsNullOrEmpty(dto.Password) && !string.IsNullOrWhiteSpace(dto.Password))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(exist);
                    var editPassword = await _userManager.ResetPasswordAsync(exist, token, dto.Password);

                    if (!editPassword.Succeeded)
                    {
                        var errors = editPassword.Errors.Select(e => e.Description).ToList();
                        return ResultService<EditUserRequest>.BadResult(
                            "Failed to update password",
                            StatusCodes.Status400BadRequest,
                            errors
                        );
                    }
                }

                var updateResult = await _userManager.UpdateAsync(exist);

                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description).ToList();
                    return ResultService<EditUserRequest>.BadResult(
                        "Failed to update user",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                if (!string.IsNullOrEmpty(dto.Role))
                {
                    var currentRole = await _userManager.GetRolesAsync(exist);
                    if (currentRole.Any())
                    {
                        var removeRole = await _userManager.RemoveFromRolesAsync(exist, currentRole);
                        if (!removeRole.Succeeded)
                        {
                            var errors = removeRole.Errors.Select(e => e.Description).ToList();
                            return ResultService<EditUserRequest>.BadResult(
                                "Failed to remove existing roles",
                                StatusCodes.Status400BadRequest,
                                errors
                            );
                        }
                    }

                    var updateRole = await _userManager.AddToRoleAsync(exist, dto.Role);
                    if (!updateRole.Succeeded)
                    {
                        var errors = updateRole.Errors.Select(e => e.Description).ToList();
                        return ResultService<EditUserRequest>.BadResult(
                            "Failed to assign new role",
                            StatusCodes.Status400BadRequest,
                            errors
                        );
                    }
                }

                var currentRoles = await _userManager.GetRolesAsync(exist);

                var responseDto = new EditUserRequest
                {
                    Id = exist.Id,
                    Email = exist.Email,
                    Name = exist.Name,
                    Role = currentRoles.ToString(),
                    LastUpdate = exist.LastUpdate,

                };

                return ResultService<EditUserRequest>.GoodResult(
                    "User updated successfully",
                    StatusCodes.Status200OK,
                    responseDto
                );
            }
            catch (Exception e)
            {
                return ResultService<EditUserRequest>.BadResult(
                    $"An error occurred while editing the user: {e.Message}",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                );
            }
        }
    }
}
