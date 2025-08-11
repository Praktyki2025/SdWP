using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SdWP.DTO.Requests;
using SdWP.Service.IServices;
using SdWP.Service.Services; 

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
    }
}