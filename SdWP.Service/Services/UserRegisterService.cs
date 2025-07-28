using Microsoft.AspNetCore.Identity;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Data.Models;
using SdWP.Service.IServices;
using Microsoft.Extensions.DependencyInjection;

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

        public async Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterRequestDTO dto)
        {
            try
            {
                var exist = await _userManager.FindByEmailAsync(dto.Email);
                if (exist != null)
                {
                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = "User with this email already exists"
                    };
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
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = $"Registration failed: {errors}"
                    };
                }

                using (var scope = _provider.CreateScope())
                {
                    var scopedUserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                    var createdUser = await scopedUserManager.FindByEmailAsync(dto.Email);
                    if (createdUser == null)
                    {
                        return new UserRegisterResponseDTO
                        {
                            Success = false,
                            Message = "User was created but could not be loaded for role assignment"
                        };
                    }

                    await scopedUserManager.AddToRoleAsync(createdUser, "User");
                    var roles = await scopedUserManager.GetRolesAsync(createdUser);

                    return new UserRegisterResponseDTO
                    {
                        Success = true,
                        Id = createdUser.Id,
                        Email = createdUser.Email,
                        Name = createdUser.Name,
                        CreatedAt = createdUser.CreatedAt,
                        Message = "User registered successfully",
                        Roles = roles.ToList()
                    };
                }
            }
            catch (Exception e)
            {
                return new UserRegisterResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred during registration: {e.Message}"
                };
            }
        }
    }
}
