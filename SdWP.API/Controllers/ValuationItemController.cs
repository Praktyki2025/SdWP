using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests.Valuation;
using SdWP.Service.IServices;
using SdWP.Service.Services;

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
    }
}
