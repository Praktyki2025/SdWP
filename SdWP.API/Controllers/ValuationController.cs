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
        public async Task<ActionResult> HandleResult<T>(ResultService<T> result)
        {
            if (result.Success)
            {
                
                if (result.StatusCode == 201)
                {
                    var idProperty = result.Data.GetType().GetProperty("Id");
                    var id = idProperty != null ? idProperty.GetValue(result.Data) : null;
                    return CreatedAtAction(nameof(GetValuationById), new { id }, result.Data);
                }
                return StatusCode(result.StatusCode, result.Data);
            }
            return StatusCode(result.StatusCode, new { result.Message, result.Errors });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllValuations()
        {
            var result = await _valuationService.GetAllValuationsAsync();
            return await HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetValuationById(Guid id)
        {
            var result = await _valuationService.GetValuationByIdAsync(id);
            return await HandleResult(result);
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetValuationsByProjectId(Guid projectId)
        {
            var result = await _valuationService.GetValuationsByProjectIdAsync(projectId);
            return await HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateValuation([FromBody] CreateValuationRequest request)
        {
            var result = await _valuationService.CreateValuationAsync(request);
            return await HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValuation(Guid id, [FromBody] UpdateValuationRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("Valuation ID mismatch.");
            }
            var result = await _valuationService.UpdateValuationAsync(request);
            return await HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValuation(Guid id)
        {
            var result = await _valuationService.DeleteValuationAsync(id);
            if (result.Success) return Ok(new { result.Message });
            return StatusCode(result.StatusCode, new { result.Message });
        }
    }
}