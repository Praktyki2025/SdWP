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

        

        public async Task<List<ValuationItemResponse>> GetValuationItemsAsync()
        {
            //pobierz dane
            var valuationItems = await _valuationItemRepository.GetValuationItemsAsync();
            //robi liste
            var valuationItemResponse = new List<ValuationItemResponse>();

            //przemapowac
            foreach (var item in valuationItems)
            {
                valuationItemResponse.Add(new ValuationItemResponse
                {
                    Name = item.Name,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalAmount = item.TotalAmount,
                   
                });
            }

            return valuationItemResponse;
            
        }

        public async Task<ValuationItemResponse> GetValuationItemByIdAsync(Guid id)
        {
         

            var item = await _valuationItemRepository.GetValuationItemByIdAsync(id); 
            var responseItem = new ValuationItemResponse
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalAmount = item.TotalAmount,
            };
            return responseItem;
        }


        public async Task<bool> DeleteValuationItemAsync(Guid id)
        {
           
            return await _valuationItemRepository.DeleteValuationItemAsync(id);
        }

    }
}
