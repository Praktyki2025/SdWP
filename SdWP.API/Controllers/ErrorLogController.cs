using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorLogController : ControllerBase
    {
        private readonly IErrorLogServices _errorLogServices;

        public ErrorLogController(IErrorLogServices errorLogServices)
        {
            _errorLogServices = errorLogServices;
        }

        [HttpPost("log")]
        public async Task<IActionResult> LogError([FromBody] ErrorLogRequest request)
        {
            var result = await _errorLogServices.GetLogToDatabase(
                request.Message,
                request.StackTrace,
                request.Source,
                request.TypeOfLog.ToString()
            );
            
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
