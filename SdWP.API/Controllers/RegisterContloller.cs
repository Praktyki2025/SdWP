using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using Microsoft.AspNetCore.Antiforgery;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class RegisterContloller : ControllerBase
    {
        private readonly IUserRegisterService _registerService;
        private readonly IAntiforgery _antiforgery;

        public RegisterContloller(IUserRegisterService registerService, IAntiforgery antiforgery)
        {
            _registerService = registerService;
            _antiforgery = antiforgery;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDTO registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new UserRegisterResponseDTO
                    {
                        Success = false,
                        Message = "Invalid input"
                    });
                }

                var response = await _registerService.RegisterAsync(registerDto);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new UserRegisterResponseDTO
                {
                    Success = false,
                    Message = $"An error occurred: {e.Message}"
                });
            }
        }
    }
}
