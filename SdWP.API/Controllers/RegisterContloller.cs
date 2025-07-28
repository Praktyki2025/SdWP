using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterContloller : ControllerBase
    {
        private readonly IUserRegisterService _registerService;
        private readonly IAntiforgery _antiforgery;
        private readonly UserManager<User> _userManager;

        public RegisterContloller(
            IUserRegisterService registerService, 
            IAntiforgery antiforgery,
            UserManager<User> userManager)
        {
            _registerService = registerService;
            _antiforgery = antiforgery;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<UserRegisterResponseDTO> RegisterAsync(UserRegisterRequestDTO dto)
        {
            try
            {
                var userExit = await _userManager.FindByEmailAsync(dto.Email);

                if (userExit != null)
                {
                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = "User with this email alredy exit"
                    };
                }

                var user = new User
                {
                    Email = dto.Email,
                    NormalizedEmail = dto.Email.Normalize(),
                    UserName = dto.Name,
                    NormalizedUserName = dto.Name.Normalize(),
                    CreatedAt = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    return new UserRegisterResponseDTO
                    {
                        Success = true,
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.Name,
                        CreatedAt = DateTime.UtcNow,
                        Message = "User registred successfully"
                    };
                }
                else
                {
                    var error = string.Join(", ", result.Errors.Select(e => e.Description));

                    return new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = $"User registration failed: {error}"
                    };
                }
            }
            catch (Exception e)
            {
                return new UserRegisterResponseDTO
                {
                    Success = false,
                    Message = $"User registration failed: {e}"
                };
            }

        }

    }
}
