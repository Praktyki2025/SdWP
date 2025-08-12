using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests.Valuation;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuationController : ControllerBase
    {
        private readonly IValuationService _valuationService;

        public ValuationController(IValuationService valuationService)
        {
            _valuationService = valuationService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetValuationList()
        {
            var result = await _valuationService.GetValuationList();
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
        public async Task<IActionResult> CreateValuation([FromBody] CreateValuationRequest request)
        {
            var result = await _valuationService.CreateValuation(request);
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpGet("cost-category")]
        public async Task<IActionResult> GetCostCategoryName()
        {
            var result = await _valuationService.GetCostCategoryName();
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpGet("cost-type")]
        public async Task<IActionResult> GetCostTypeName()
        {
            var result = await _valuationService.GetCostTypeName();
            return result.Success
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
        }

        [HttpGet("user-group")]
        public async Task<IActionResult> GetUserGroupName()
        {
            var result = await _valuationService.GetUserGroupName();
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