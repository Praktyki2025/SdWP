using Microsoft.AspNetCore.Identity;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Data.Models;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class UserRegisterService : IUserRegisterService
    {
        private readonly UserManager<User> _userManager;

        public UserRegisterService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterRequestDTO dto)
        {
            try
            {
                var userExist = await _userManager.FindByEmailAsync(dto.Email);

                if (userExist != null)
                {
                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = "User already exists with this email."
                    };
                }

                var user = new User
                {
                    Email = dto.Email,
                    UserName = dto.Name.ToUpper(),
                    Name = dto.Name,
                    CreatedAt = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    return new UserRegisterResponseDTO
                    {
                        Success = true,
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        CreatedAt = user.CreatedAt,
                        Message = "User registered successfully."
                    };
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));

                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = $"User registration failed: {errors}"
                    };
                }
            }
            catch (Exception e)
            {
                return new UserRegisterResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred while registering the user: {e.Message}"
                };
            }
        }
    }
}
