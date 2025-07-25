using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Data.Models;
using Microsoft.AspNetCore.Identity;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class UserLoginServices : IUserLoginService
    {
        private readonly UserManager<User> _userManager;

        public UserLoginServices(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<UserLoginResponseDTO?> LoginAsync(UserLoginRequestDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null; 
            
            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isValid) return null;

            return new UserLoginResponseDTO
            {
                Success = true,
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                LoginTime = DateTime.UtcNow
            };
        }
    }
}
