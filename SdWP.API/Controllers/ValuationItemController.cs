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

            var valuationItem = _valuationItemService.GetValuationItemsAsync();
            return Ok(valuationItem);
        }

    }
}
