using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using SdWP.Data.Interfaces;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class ValuationItemService : IValuationItemService
    {
        private readonly IValuationItemRepository _valuationItemRepository;


        public ValuationItemService(IValuationItemRepository valuationItemRepository)
        {
            _valuationItemRepository = valuationItemRepository;
        }

        public Task<List<ValuationItemResponse>> GetValuationItemByThingsIdAsync(Guid id)
        {

            throw new NotImplementedException();
        }

   

        public async Task<List<ValuationItemResponse>> GetValuationItemsAsync()
        {
            var valuationItems = await _valuationItemRepository.GetValuationItemsAsync();
            var valuationItemResponse = new List<ValuationItemResponse>();

        //przemapowac
            return valuationItemResponse;
            
            throw new NotImplementedException();
        }
    }
}
