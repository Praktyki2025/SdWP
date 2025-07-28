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

        public async Task<User?> ValidateUserAsync(UserLoginRequestDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return null;

            var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            return isValid ? user : null;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
