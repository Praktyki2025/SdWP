using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("log/Message={errorMessage}StackTrace={stackTrace}Source={source}TypeOfLog={typeOfLog}")]
        public async Task<IActionResult> LogError(
            string errorMessage,
            string stackTrace,
            string source,
            string typeOfLog
            )
        {
            var result = await _errorLogServices.GetLogToDatabase(
                errorMessage,
                stackTrace,
                source,
                typeOfLog
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
