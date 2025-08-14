using Microsoft.AspNetCore.Http;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Requests.Datatable;
using SdWP.DTO.Responses;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;
using SdWP.DTO.Responses.DataTable;
using SdWP.DTO.Requests.Mailing;
using SdWP.Data.IData;

namespace SdWP.Service.Services
{
    public class UserService : IUserService
    {
        // In this services class, we handle user registration, delete, and edit user data.

        private readonly IUserRepository _userRepository;
        private readonly IErrorLogHelper _errorLogServices;

        private string message = string.Empty;

        public UserService(IUserRepository userRepository, IErrorLogHelper errorLogServices)
        {
            _userRepository = userRepository;
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<AddUserResponse>> RegisterAsync(AddUserRequest dto)
        {
            try
            {
                var exist = await _userRepository.FindByEmailAsync(dto.Email);
                if (exist != null)
                {

                    message = $"User registration attempt with existing email: {dto.Email}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                    ));
                }

                if (string.IsNullOrEmpty(dto.Role))
                {
                    message = $"User registration attempt with unknown role {dto.Role}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };



                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                    ));
                }

                if (!await _userRepository.RoleExistsAsync(dto.Role))
                {
                    message = $"User registration attempt with unknown role {dto.Role}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "UserServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
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

                var result = await _userRepository.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();

                    message = $"User creation failed for email: {dto.Email}, Errors: {errors}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
                }

                var createdUser = await _userRepository.FindByEmailAsync(dto.Email);
                if (createdUser == null)
                {
                    message = $"User created but could not be loaded for role assignment: {dto.Email}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                    ));
                }

                var roleResult = await _userRepository.AddToRoleAsync(createdUser, dto.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    message = $"User creation failed for email: {dto.Email}, Errors: {errors}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest,
                        errors
                    ));
                }

                var roles = await _userRepository.GetRolesAsync(createdUser);
                if (roles == null || !roles.Any())
                {
                    message = $"User created but no roles assigned for email: {dto.Email}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LoginServices.RegisterAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                    ));
                }

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
                message = $"Error during user registration: {e.Message}";
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
                    .ContinueWith(_ => ResultService<AddUserResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<DataTableResponse<UserListResponse>>> GetUserListAsync(DataTableRequest request)
        {
            try
            {
                var users = await _userRepository.GetUsersAsync(request);

                if (users == null || users.Count == 0)
                {
                    message = "No users found";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/GetUserListAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO).
                        ContinueWith(_ => ResultService<DataTableResponse<UserListResponse>>.BadResult(
                            message,
                            StatusCodes.Status404NotFound
                    ));
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
                message = $"An error occurred while retrieving users: {e.Message}";
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
                    .ContinueWith(_ => ResultService<DataTableResponse<UserListResponse>>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError,
                        new List<string> { e.Message }
                ));
            }
        }

        public async Task<ResultService<UserListResponse>> DeleteUserAsync(DeleteUserRequest dto)
        {
            try
            {
                var user = await _userRepository.FindByIdAsync(dto.Id.ToString());
                if (user == null)
                {
                    message = $"User deletion attempt for non-existing user ID: {dto.Id}";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponse>.BadResult(
                        message,
                        StatusCodes.Status404NotFound
                    ));
                }

                var roles = await _userRepository.GetRolesAsync(user); ;

                if (roles != null && roles.Contains("Admin"))
                {
                    message = $"Attempt to delete admin user: {user.Email}";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponse>.BadResult(
                        message,
                        StatusCodes.Status403Forbidden
                    ));
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

                var result = await _userRepository.DeleteAsync(user);

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
                    message = $"User deletion failed for user ID: {dto.Id}, Errors: {errors}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "backend",
                        Source = "LoginServices.DeleteUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponse>.BadResult(
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

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = "LoginServices.DeleteUserAsync",
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Warning
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserListResponse>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError,
                    new List<string> { e.Message }
                ));
            }
        }

        public async Task<ResultService<EditUserRequest>> EditUserAsync(EditUserRequest dto)
        {
            try
            {
                var exist = await _userRepository.FindByIdAsync(dto.Id.ToString());
                if (exist == null)
                {
                    message = $"User edit attempt for non-existing user ID: {dto.Id}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/EditUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                    ));
                }

                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != exist.Email)
                {
                    var userWithEmail = await _userRepository.FindByEmailAsync(dto.Email);
                    if (userWithEmail != null && userWithEmail.Id != exist.Id)
                    {
                        message = $"User edit attempt with existing email: {dto.Email}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Unknow",
                            Source = "UserServices/EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                        ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != exist.Name)
                {
                    var userWithName = await _userRepository.FindByNameAsync(dto.Name);
                    if (userWithName != null && userWithName.Id != exist.Id)
                    {
                        message = $"User edit attempt with existing name: {dto.Name}";
                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Unknow",
                            Source = "UserServices/EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                            ));
                    }
                }

                if (!string.IsNullOrEmpty(dto.Role) && !await _userRepository.RoleExistsAsync(dto.Role))
                {
                    message = $"Role not exist: {dto.Role}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/EditUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                if (dto.IsLocked.HasValue)
                {
                    var enableResult = await _userRepository.SetLockoutEnabledAsync(exist, true);
                    if (!enableResult.Succeeded)
                    {
                        var errors = enableResult.Errors.Select(e => e.Description).ToList();
                        return ResultService<EditUserRequest>.BadResult(
                            "Failed to lock user account",
                            StatusCodes.Status400BadRequest,
                            errors
                        );
                    }

                    if (dto.IsLocked.Value)
                    {
                        var lockResult = await _userRepository.SetLockoutEndDateAsync(exist, DateTimeOffset.MaxValue);
                        if (!lockResult.Succeeded)
                        {
                            var errors = lockResult.Errors.Select(e => e.Description).ToList();
                            return ResultService<EditUserRequest>.BadResult(
                                "Failed to lock user account",
                                StatusCodes.Status400BadRequest,
                                errors
                            );
                        }
                    }
                    else
                    {
                        var unlockResult = await _userRepository.SetLockoutEndDateAsync(exist, null);
                        if (!unlockResult.Succeeded)
                        {
                            var errors = unlockResult.Errors.Select(e => e.Description).ToList();
                            return ResultService<EditUserRequest>.BadResult(
                                "Failed to unlock user account",
                                StatusCodes.Status400BadRequest,
                                errors
                            );
                        }
                    }
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
                    var token = await _userRepository.GeneratePasswordResetTokenAsync(exist);
                    var editPassword = await _userRepository.ResetPasswordAsync(exist, token, dto.Password);

                    if (!editPassword.Succeeded)
                    {
                        var errors = editPassword.Errors.Select(e => e.Description).ToList();
                        message = $"Failed to update password: {errors}";
                        Log.Error(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Unknow",
                            Source = "UserServices/EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Error
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                            ));
                    }
                }

                var updateResult = await _userRepository.UpdateAsync(exist);
                if (!updateResult.Succeeded)
                {
                    var errors = updateResult.Errors.Select(e => e.Description).ToList();
                    message = $"Failed to update user: {errors}";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/EditUserAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                if (!string.IsNullOrEmpty(dto.Role))
                {
                    var currentRole = await _userRepository.GetRolesAsync(exist);
                    if (currentRole.Any())
                    {
                        var removeRole = await _userRepository.RemoveFromRolesAsync(exist, currentRole);
                        if (!removeRole.Succeeded)
                        {
                            var errors = removeRole.Errors.Select(e => e.Description).ToList();
                            message = $"Failed to removing existing roles: {errors}";
                            Log.Error(message);

                            var errorLogDTO = new ErrorLogResponse
                            {
                                Id = Guid.NewGuid(),
                                Message = message,
                                StackTrace = "Unknow",
                                Source = "UserServices/EditUserAsync",
                                TimeStamp = DateTime.UtcNow,
                                TypeOfLog = TypeOfLog.Error
                            };

                            return await _errorLogServices.LoggEvent(errorLogDTO)
                                .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                                    message,
                                    StatusCodes.Status400BadRequest
                                ));
                        }
                    }

                    var updateRole = await _userRepository.AddToRoleAsync(exist, dto.Role);
                    if (!updateRole.Succeeded)
                    {
                        var errors = updateRole.Errors.Select(e => e.Description).ToList();
                        message = $"Failed to asign new role: {errors}";
                        Log.Error(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Unknow",
                            Source = "UserServices/EditUserAsync",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Error
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                            ));
                    }
                }

                exist = await _userRepository.FindByIdAsync(exist.Id.ToString());
                var currentRoles = await _userRepository.GetRolesAsync(exist);

                var responseDto = new EditUserRequest
                {
                    Id = exist.Id,
                    Email = exist.Email,
                    Name = exist.Name,
                    Role = currentRoles.ToString(),
                    LastUpdate = exist.LastUpdate,
                    IsLocked = exist.LockoutEnd.HasValue && exist.LockoutEnd.Value > DateTimeOffset.UtcNow,

                };

                return ResultService<EditUserRequest>.GoodResult(
                    "User updated successfully",
                    StatusCodes.Status200OK,
                    responseDto
                );
            }
            catch (Exception e)
            {
                message = $"An error occurred while editing the user: {e.Message}";
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
                    .ContinueWith(_ => ResultService<EditUserRequest>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError,
                        new List<string> { e.Message }
                ));
            }
        }

        public async Task<ResultService<string>> ResetPasswordAsync(ResetPasswordRequest dto)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    message = "No users found";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/ResetPasswordAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO).
                        ContinueWith(_ => ResultService<string>.BadResult(
                            message,
                            StatusCodes.Status404NotFound
                    ));
                }

                var token = Uri.UnescapeDataString(dto.Token);
                var result = await _userRepository.ResetPasswordAsync(user, token, dto.Password);

                if (result.Succeeded)
                {
                    return ResultService<string>.GoodResult(
                        "Password reset successfullt",
                        StatusCodes.Status200OK
                        );
                }
                else
                {
                    message = $"Password reset failed: {string.Join(", ", result.Errors.Select(e => e.Description))}";

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "UserServices/GetUserListAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<string>.BadResult(
                            message,
                            StatusCodes.Status404NotFound
                        ));
                }
            }
            catch (Exception e)
            {
                message = $"An error occurred while change password: {e.Message}";
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
                    .ContinueWith(_ => ResultService<string>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError,
                        new List<string> { e.Message }
                ));
            }
        }

    }
}
