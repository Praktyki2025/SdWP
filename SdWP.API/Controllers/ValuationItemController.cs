using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests.Valuation;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuationItemController : ControllerBase
    {
        private readonly IValuationItemService _valuationItemService;
        public ValuationItemController(IValuationItemService valuationItemService)
        {
            _valuationItemService = valuationItemService ?? throw new ArgumentNullException(nameof(valuationItemService));
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetValuationList([FromServices] IValuationItemService valuationItemService)
        {
            var result = await valuationItemService.GetValuationList();
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteValuationItem(Guid id)
        {
            var result = await _valuationItemService.DeleteValuationItem(id);
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateValuationItem([FromBody] CreateValuationItemRequest request)
        {
            var result = await _valuationItemService.CreateValuationItem(request);
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
        public async Task<IActionResult> UpdateValuationItem([FromBody] UpdateValuationItemRequest request)
        {
            var result = await _valuationItemService.UpdateValuationItem(request);
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
