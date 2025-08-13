using AutoMapper;
using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace SdWP.Service.Services
{
    public class ValuationItemService : IValuationItemService
    {
        private readonly IValuationItemRepository _valuationItemRepository;

        public ValuationItemService(IValuationItemRepository valuationItemRepository)
        {
            _valuationItemRepository = valuationItemRepository ?? throw new ArgumentNullException(nameof(valuationItemRepository));
        }

        public async Task<ResultService<List<ValuationItemResponse>>> GetValuationList()
        {
            try
            {
                var valuationItems = await _valuationItemRepository.GetAllValuationItemsAsync();

                if (valuationItems == null)
                {
                    return ResultService<List<ValuationItemResponse>>.BadResult(
                        "Valuation item not found.",
                        StatusCodes.Status404NotFound);
                }

                var response = valuationItems.Select(vi => new ValuationItemResponse
                {
                    Id = vi.Id,
                    Name = vi.Name,
                    Description = vi.Description,
                    Quantity = vi.Quantity,
                    UnitPrice = vi.UnitPrice,
                    TotalAmount = vi.TotalAmount
                }).ToList();

                return ResultService<List<ValuationItemResponse>>.GoodResult(
                    "Valuation items retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<List<ValuationItemResponse>>.BadResult(
                    $"An error occurred: {ex.Message} | throw {ex.InnerException}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<DeleteValuationItemResponse>> DeleteValuationItem(Guid id)
        {
            try
            {
                var valuationItem = await _valuationItemRepository.GetValuationItemByIdAsync(id);

                if (valuationItem == null)
                {
                    return ResultService<DeleteValuationItemResponse>.BadResult(
                        "Valuation item not found.",
                        StatusCodes.Status404NotFound);
                }

                await _valuationItemRepository.DeleteValuationItemAsync(id);

                return ResultService<DeleteValuationItemResponse>.GoodResult(
                    "Valuation item deleted successfully.",
                    StatusCodes.Status200OK,
                    new DeleteValuationItemResponse { Id = id });
            }
            catch (Exception ex)
            {
                return ResultService<DeleteValuationItemResponse>.BadResult(
                    $"An error occurred: {ex.Message} | throw {ex.InnerException}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}