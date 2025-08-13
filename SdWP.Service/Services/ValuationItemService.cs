using AutoMapper;
using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
using SdWP.Data.Models;
using SdWP.Data.Repositories;
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
        private readonly ICostTypeRepository _costTypeRepository;
        private readonly ICostCategoryRepsoitory _costCategoryRepsoitory;
        private readonly IUserGroupTypeRepository _userGroupTypeRepository;

        public ValuationItemService(
            IValuationItemRepository valuationItemRepository,
            ICostTypeRepository costTypeRepository,
            ICostCategoryRepsoitory costCategoryRepsoitory,
            IUserGroupTypeRepository userGroupTypeRepository
            )
        {
            _valuationItemRepository = valuationItemRepository;
            _costTypeRepository = costTypeRepository;
            _costCategoryRepsoitory = costCategoryRepsoitory;
            _userGroupTypeRepository = userGroupTypeRepository;

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

        public async Task<ResultService<CreateValuationItemResponse>> CreateValuationItem(CreateValuationItemRequest request)
        {
            try 
            {
                var costTypeId = await _costTypeRepository.GetGuidByName(request.CostTypeName);
                if (costTypeId == Guid.Empty)
                {
                    return ResultService<CreateValuationItemResponse>.BadResult(
                        "Cost type not found.",
                        StatusCodes.Status404NotFound);
                }

                var coastCategoryId = await _costCategoryRepsoitory.GetGuidByName(request.CostCategoryName);
                if (coastCategoryId == Guid.Empty)
                {
                    return ResultService<CreateValuationItemResponse>.BadResult(
                        "Cost category not found.",
                        StatusCodes.Status404NotFound);
                }

                var userGroupTypeId = await _userGroupTypeRepository.GetGuidByName(request.UserGroupTypeName);
                if (userGroupTypeId == Guid.Empty)
                {
                    return ResultService<CreateValuationItemResponse>.BadResult(
                        "User group type not found.",
                        StatusCodes.Status404NotFound);
                }

                var valuationItem = new CreateValuationItemResponse
                {
                    ValuationId = request.ValuationId,
                    Name = request.Name,
                    Description = request.Description,
                    CreatorUserId = request.CreatorUserId,
                    CostTypeId = costTypeId,
                    CostCategoryID = coastCategoryId,
                    UserGroupTypeId = userGroupTypeId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalAmount = request.TotalAmount,
                    RecurrencePeriod = request.RecurrencePeriod,
                    RecurrenceUnit = request.RecurrenceUnit,
                };

                var result = await _valuationItemRepository.AddValuationItemAsync(valuationItem);
                if (result == null)
                {
                    return ResultService<CreateValuationItemResponse>.BadResult(
                        "Failed to create valuation item.",
                        StatusCodes.Status500InternalServerError);
                }

                return ResultService<CreateValuationItemResponse>.GoodResult(
                    "Valuation item created successfully.",
                    StatusCodes.Status201Created,
                    valuationItem);
            }
            catch (Exception ex)
            {
                return ResultService<CreateValuationItemResponse>.BadResult(
                    $"An error occurred: {ex.Message} | throw {ex.InnerException}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}