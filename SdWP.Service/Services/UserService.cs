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
    public class UserService : IUserService
    {
        // In this services class, we handle user registration, delete, and edit user data.

        private readonly UserManager<User> _userManager;
        private readonly UserRepository _userRepository;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IErrorLogHelper _errorLogServices;

        private string message = string.Empty;


        public UserService(
            UserManager<User> userManager,
            UserRepository userRepository,
            RoleManager<IdentityRole<Guid>> roleManager,
            IErrorLogHelper errorLogServices)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleManager = roleManager;
            _errorLogServices = _errorLogServices;
        }

        public async Task<ResultService<AddUserResponseDTO>> RegisterAsync(AddUserRequestDTO dto)
        {
            try
            {
                var exist = await _userManager.FindByEmailAsync(dto.Email);
                if (exist != null)
                {
                    message = $"User registration attempt with existing email: {dto.Email}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

                if (string.IsNullOrEmpty(dto.Role))
                {
                    message = $"User registration attempt with unknown role {dto.Role}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };



                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    message = $"User registration attempt with unknown role {dto.Role}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
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

                    message = $"User creation failed for email: {dto.Email}, Errors: {errors}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
                }

                var createdUser = await _userManager.FindByEmailAsync(dto.Email);
                if (createdUser == null)
                {
                    message = $"User created but could not be loaded for role assignment: {dto.Email}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

                var roleResult = await _userManager.AddToRoleAsync(createdUser, dto.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    message = $"User creation failed for email: {dto.Email}, Errors: {errors}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
                }

                var roles = await _userManager.GetRolesAsync(createdUser);
                if (roles == null || !roles.Any())
                {
                    message = $"User created but no roles assigned for email: {dto.Email}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

                var responseDto = new AddUserResponseDTO
                {
                    Success = true,
                    Id = createdUser.Id,
                    Email = createdUser.Email,
                    Name = createdUser.Name,
                    CreatedAt = createdUser.CreatedAt,
                    Message = "User registered successfully",
                    Roles = roles.ToList()
                };

                return ResultService<AddUserResponseDTO>.GoodResult(
                    "User registered successfully",
                    statusCode: StatusCodes.Status201Created,
                    responseDto
                );
            }
            catch (Exception e)
            {
                message = $"Error during user registration: {e.Message}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "UserServices.RegisterAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Error
                };


                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<AddUserResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<List<UserListResponseDTO>>> GetUserListAsync()
        {
            try
            {
                var users = await _userRepository.GetUserRoleAsync(CancellationToken.None);

                if (users == null)
                {
                    message = ($"No users found in the database.");
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "UserServices.GetUserListAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<List<UserListResponseDTO>>.BadResult(
                        message,
                        StatusCodes.Status404NotFound
                    ));
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
                        message,
                        StatusCodes.Status200OK,
                        userList
                );
            }
            catch (Exception e)
            {

                message = $"Error retrieving user list: {e.Message}";

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "UserServices.GetUserListAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<List<UserListResponseDTO>>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                ));
            }
        }

        public async Task<ResultService<UserListResponseDTO>> DeleteUserAsync(DeleteUserRequestDTO dto)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(dto.Id.ToString(), CancellationToken.None);
                if (user == null)
                {
                    message = $"User deletion attempt for non-existing user ID: {dto.Id}";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status404NotFound
                    ));
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null && roles.Contains("Admin"))
                {
                    message = $"Attempt to delete admin user: {user.Email}";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status403Forbidden
                    ));
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
                    message = $"User deletion failed for user ID: {dto.Id}, Errors: {errors}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponseDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
                }
            }
            catch (Exception e)
            {
                message = $"Error during user deletion: {e.Message}";

                Log.Error(message);

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.DeleteUserAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Warning
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponseDTO>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                ));
            }
        }

        public async Task<ResultService<EditUserRequestDTO>> EditUserAsync(EditUserRequestDTO dto)
        {
            try
            {
                var exist = await _userManager.FindByIdAsync(dto.Id.ToString());
                if (exist == null)
                {
                    message = $"User edit attempt for non-existing user ID: {dto.Id}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.EditUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != exist.Email)
                {
                    var userWithEmail = await _userManager.FindByEmailAsync(dto.Email);
                    if (userWithEmail != null && userWithEmail.Id != exist.Id)
                    {
                        message = $"User edit attempt with existing email: {dto.Email}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponseDTO
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "backend",
                            Source = "LoginServices.EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLogEnum.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != exist.Name)
                {
                    var userWithName = await _userManager.FindByNameAsync(dto.Name);
                    if (userWithName != null && userWithName.Id != exist.Id)
                    {
                        message = $"User edit attempt with existing name: {dto.Name}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponseDTO
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "backend",
                            Source = "LoginServices.EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLogEnum.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.Role) && !await _roleManager.RoleExistsAsync(dto.Role))
                {
                    message = "User edit attempt with unknown role: {dto.Role}";

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.EditUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
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
                        message = $"User password update failed for user ID: {dto.Id}, Errors: {string.Join(", ", errors)}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponseDTO
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "backend",
                            Source = "LoginServices.EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLogEnum.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest,
                            errors
                        ));
                    }
                }

                var updateResult = await _userManager.UpdateAsync(exist);

                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description).ToList();
                    message = $"User update failed for user ID: {dto.Id}, Errors: {string.Join(", ", errors)}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponseDTO
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.HandleLogoutAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLogEnum.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
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
                            message = $"Failed to remove existing roles for user ID: {dto.Id}, Errors: {string.Join(", ", errors)}";
                            Log.Warning(message);

                            var errorLogDTO = new ErrorLogResponseDTO
                            {
                                Id = Guid.NewGuid(),
                                Message = message,
                                StackTrace = message,
                                Source = "LoginServices.HandleLogoutAsync",
                                TimeStamp = DateTime.UtcNow,
                                TypeOfLog = TypeOfLogEnum.Warning
                            };

                            return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest,
                                errors
                            ));
                        }
                    }

                    var updateRole = await _userManager.AddToRoleAsync(exist, dto.Role);
                    if (!updateRole.Succeeded)
                    {
                        var errors = updateRole.Errors.Select(e => e.Description).ToList();
                        message = $"Failed to assign new role for user ID: {dto.Id}, Errors: {string.Join(", ", errors)}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponseDTO
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "backend",
                            Source = "LoginServices.HandleLogoutAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLogEnum.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest,
                            errors
                        ));
                    }
                }

                var currentRoles = await _userManager.GetRolesAsync(exist);

                var responseDto = new EditUserRequestDTO
                {
                    Id = exist.Id,
                    Email = exist.Email,
                    Name = exist.Name,
                    Role = currentRoles.ToString(),
                    LastUpdate = exist.LastUpdate,

                };

                return ResultService<EditUserRequestDTO>.GoodResult(
                    "User updated successfully",
                    StatusCodes.Status200OK,
                    responseDto
                );
            }
            catch (Exception e)
            {
                message = $"Error during user edit: {e.Message}";
                Log.Warning(message);

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.EditUserAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Warning
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<EditUserRequestDTO>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                ));
            }
        }
    }
}
