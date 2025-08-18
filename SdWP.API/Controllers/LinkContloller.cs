using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests.Valuation;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/link")]
    public class LinkContloller : ControllerBase
    {
        private readonly ILinkServices _linkServices;

        public LinkContloller(ILinkServices linkServices)
        {
            _linkServices = linkServices;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddLinkAsync([FromBody] AddLinkRequest request)
        {
            var result = await _linkServices.AddLinkAsync(request);

            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpGet("project/{id:guid}")]
        public async Task<IActionResult> GetAllLinksToProjectAsync(Guid id)
        {
            var result = await _linkServices.GetLinkByProjectId(id);
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteLinkAsync(Guid id)
        {
            var result = await _linkServices.DeleteLinkAsync(id);
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLinkAsync([FromBody] UpdateLinkRequest request)
        {
            var result = await _linkServices.UpdateLinkAsync(request);

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