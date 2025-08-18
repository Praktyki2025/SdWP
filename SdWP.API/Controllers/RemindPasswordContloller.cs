using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests.Mailing;
using SdWP.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class RemindPasswordContloller : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public RemindPasswordContloller(IEmailService emailService, IUserService userService)
        {
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost("remind-password")]
        public async Task<ActionResult> RemindPasswordAsync([FromBody] RemindPasswordRequest dto)
        {
            var result = await _emailService.SendPasswordResetEmailAsync(dto);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest dto)
        {
            var result = await _userService.ResetPasswordAsync(dto);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }
    }

}
