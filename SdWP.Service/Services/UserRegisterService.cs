using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class UserRegisterService : IUserRegisterService
    {
        private readonly UserManager<User> _userManager;
        private readonly IServiceProvider _provider;

        public UserRegisterService(
            UserManager<User> userManager,
            IServiceProvider provider)
        {
            _userManager = userManager;
            _provider = provider;
        }

        public async Task<ResultService<UserRegisterResponseDTO>> RegisterAsync(UserRegisterRequestDTO dto)
        {
            try
            {
                var exist = await _userManager.FindByEmailAsync(dto.Email);
                if (exist != null)
                {
                    return ResultService<UserRegisterResponseDTO>.BadResult(
                        null,
                        "User with this email already exists",
                        409
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
                    LastUpdate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ResultService<UserRegisterResponseDTO>.BadResult(
                        null,
                        "User creation failed",
                        400,
                        errors
                    );
                }

                using (var scope = _provider.CreateScope())
                {
                    var scopedUserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var createdUser = await scopedUserManager.FindByEmailAsync(dto.Email);
                    if (createdUser == null)
                    {
                        return ResultService<UserRegisterResponseDTO>.BadResult(
                            null,
                            "User was created but could not be loaded for role assignment",
                            500
                        );
                    }

                    await scopedUserManager.AddToRoleAsync(createdUser, "User");
                    var roles = await scopedUserManager.GetRolesAsync(createdUser);

                    var responseDto = new UserRegisterResponseDTO
                    {
                        Success = true,
                        Id = createdUser.Id,
                        Email = createdUser.Email,
                        Name = createdUser.Name,
                        CreatedAt = createdUser.CreatedAt,
                        Message = "User registered successfully",
                        Roles = roles.ToList()
                    };

                    return ResultService<UserRegisterResponseDTO>.GoodResult(
                        responseDto,
                        "User registered successfully",
                        201
                    );
                }
            }
            catch (Exception e)
            {
                return ResultService<UserRegisterResponseDTO>.BadResult(
                        null,
                        $"An error occurred during registration: {e.Message}",
                        500
                    );
            }
        }
    }
}
