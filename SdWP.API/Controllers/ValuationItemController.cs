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
    public class ValuationItemController : ControllerBase
    {
        private readonly IValuationItemService _valuationItemService;

        public ValuationItemController(IValuationItemService valuationItemService)
        {
            _valuationItemService = valuationItemService;
        }

       
        private ActionResult HandleResult<T>(ResultService<T> result)
        {
            if (result.Success)
            {
                
                if (result.StatusCode == 201)
                {
                    var idProperty = result.Data.GetType().GetProperty("Id");
                    var id = idProperty != null ? idProperty.GetValue(result.Data) : null;
                    return CreatedAtAction(nameof(GetValuationItemById), new { id }, result.Data);
                }
                return StatusCode(result.StatusCode, result.Data);
            }
            return StatusCode(result.StatusCode, new { result.Message, result.Errors });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllValuationItems()
        {
            var result = await _valuationItemService.GetAllValuationItemsAsync();
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetValuationItemById(Guid id)
        {
            var result = await _valuationItemService.GetValuationItemByIdAsync(id);
            return HandleResult(result);
        }

        [HttpGet("valuation/{valuationId}")]
        public async Task<IActionResult> GetValuationItemsByValuationId(Guid valuationId)
        {
            var result = await _valuationItemService.GetValuationItemsByValuationIdAsync(valuationId);
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateValuationItem([FromBody] CreateValuationItemRequest request)
        {
            var result = await _valuationItemService.CreateValuationItemAsync(request);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValuationItem(Guid id, [FromBody] UpdateValuationItemRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("Valuation Item ID mismatch.");
            }
            var result = await _valuationItemService.UpdateValuationItemAsync(request);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValuationItem(Guid id)
        {
            var result = await _valuationItemService.DeleteValuationItemAsync(id);
            
            if (result.Success) return Ok(new { result.Message });
            return StatusCode(result.StatusCode, new { result.Message });
        }
    }
}