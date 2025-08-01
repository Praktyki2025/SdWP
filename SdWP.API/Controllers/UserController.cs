using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;


namespace SdWP.API.Controllers
{


    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _registerService;

        public UserController(IUserService registerService)
        {
            _registerService = registerService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterAsync(RegisterRequestDTO dto)
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
