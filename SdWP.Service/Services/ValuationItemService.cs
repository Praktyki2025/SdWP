using AutoMapper;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using SdWP.Service.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SdWP.Service.Services
{
    public class ValuationItemService : IValuationItemService
    {
        private readonly IValuationItemRepository _valuationItemRepository;
        private readonly IMapper _mapper;

        public ValuationItemService(IValuationItemRepository valuationItemRepository, IMapper mapper)
        {
            _valuationItemRepository = valuationItemRepository;
            _mapper = mapper;
        }

        public async Task<ResultService<ValuationItemResponse>> CreateValuationItemAsync(CreateValuationItemRequest request)
        {
            try
            {
                var valuationItem = _mapper.Map<ValuationItem>(request);
                valuationItem.Id = Guid.NewGuid();
                valuationItem.CreatedAt = DateTime.UtcNow;
                valuationItem.LastModified = DateTime.UtcNow;

                var createdValuationItem = await _valuationItemRepository.AddValuationItemAsync(valuationItem);
                var response = _mapper.Map<ValuationItemResponse>(createdValuationItem);

                return ResultService<ValuationItemResponse>.GoodResult(
                    "Valuation item created successfully.",
                    StatusCodes.Status201Created,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationItemResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<string>> DeleteValuationItemAsync(Guid id)
        {
            try
            {
                var existingItem = await _valuationItemRepository.GetValuationItemByIdAsync(id);
                if (existingItem == null)
                {
                    return ResultService<string>.BadResult(
                        "Valuation item not found.",
                        StatusCodes.Status404NotFound);
                }

                await _valuationItemRepository.DeleteValuationItemAsync(id);
                return ResultService<string>.GoodResult(
                    "Valuation item deleted successfully.",
                    StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ResultService<string>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<IEnumerable<ValuationItemResponse>>> GetAllValuationItemsAsync()
        {
            try
            {
                var valuationItems = await _valuationItemRepository.GetAllValuationItemsAsync();
                var response = _mapper.Map<IEnumerable<ValuationItemResponse>>(valuationItems);
                return ResultService<IEnumerable<ValuationItemResponse>>.GoodResult(
                    "Valuation items retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<ValuationItemResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<ValuationItemResponse>> GetValuationItemByIdAsync(Guid id)
        {
            try
            {
                var valuationItem = await _valuationItemRepository.GetValuationItemByIdAsync(id);
                if (valuationItem == null)
                {
                    return ResultService<ValuationItemResponse>.BadResult(
                        "Valuation item not found.",
                        StatusCodes.Status404NotFound);
                }

                var response = _mapper.Map<ValuationItemResponse>(valuationItem);
                return ResultService<ValuationItemResponse>.GoodResult(
                    "Valuation item retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationItemResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<IEnumerable<ValuationItemResponse>>> GetValuationItemsByValuationIdAsync(Guid valuationId)
        {
            try
            {
                var valuationItems = await _valuationItemRepository.GetValuationItemsByValuationIdAsync(valuationId);
                var response = _mapper.Map<IEnumerable<ValuationItemResponse>>(valuationItems);
                return ResultService<IEnumerable<ValuationItemResponse>>.GoodResult(
                    "Valuation items retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<ValuationItemResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }
        public async Task<ResultService<ValuationItemResponse>> UpdateValuationItemAsync(UpdateValuationItemRequest request)
        {
            try
            {
                var existingItem = await _valuationItemRepository.GetValuationItemByIdAsync(request.Id);
                if (existingItem == null)
                {
                    return ResultService<ValuationItemResponse>.BadResult(
                        "Valuation item not found.",
                        StatusCodes.Status404NotFound);
                }

                _mapper.Map(request, existingItem);
                existingItem.LastModified = DateTime.UtcNow;

                var updatedItem = await _valuationItemRepository.UpdateValuationItemAsync(existingItem);
                var response = _mapper.Map<ValuationItemResponse>(updatedItem);

                return ResultService<ValuationItemResponse>.GoodResult(
                    "Valuation item updated successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationItemResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}