using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{

    [ApiController]
    [Route("api/register")]
    public class RegisterController : ControllerBase
    {
        private readonly IUserRegisterService _registerService;

        public RegisterController(IUserRegisterService registerService)
        {
            _registerService = registerService;
        }

        public async Task<ActionResult> RegisterAsync(UserRegisterRequestDTO dto)
        {
            var result = await _registerService.RegisterAsync(dto);

            if (result.Success) return StatusCode( result.StatusCode, result.Data);

            return StatusCode( result.StatusCode, new
            {
                success = false,
                message = result.Message,
                errors = result.Errors
            });
        }

    }
}
