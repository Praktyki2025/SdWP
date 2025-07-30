using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SdWP.Service.IServices;

namespace SdWP.API.Controllers
{

    [ApiController]
    public class ValuationItemController : ControllerBase
    {
        private readonly IValuationItemService _valuationItemService;

        public ValuationItemController(IValuationItemService valuationItemService)
        {
            _valuationItemService = valuationItemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetValuationItems()
        {

            var valuationItem = await _valuationItemService.GetValuationItemsAsync();
            return Ok(valuationItem);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetValuationItemById(Guid id)
        {
            var valuationItem = await _valuationItemService.GetValuationItemByIdAsync(id);
            if (valuationItem == null)
            {
                return NotFound(); 
            }
            return Ok(valuationItem);
        }



        [HttpDelete] 
        public async Task<IActionResult> DeleteValuationItem(Guid id)
        {
            var deleted = await _valuationItemService.DeleteValuationItemAsync(id);
            return Ok(deleted);
        }



    }
}
