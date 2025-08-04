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
    public class ValuationService : IValuationService
    {
        private readonly IValuationRepository _valuationRepository;
        private readonly IMapper _mapper;

        public ValuationService(IValuationRepository valuationRepository, IMapper mapper)
        {
            _valuationRepository = valuationRepository;
            _mapper = mapper;
        }

        public async Task<ResultService<ValuationResponse>> CreateValuationAsync(CreateValuationRequest request)
        {
            try
            {
                var valuation = _mapper.Map<Valuation>(request);
                valuation.Id = Guid.NewGuid();
                valuation.CreatedAt = DateTime.UtcNow;
                valuation.LastModified = DateTime.UtcNow;

                var createdValuation = await _valuationRepository.AddValuationAsync(valuation);
                var response = _mapper.Map<ValuationResponse>(createdValuation);

                return ResultService<ValuationResponse>.GoodResult(
                    "Valuation created successfully.",
                    StatusCodes.Status201Created,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<string>> DeleteValuationAsync(Guid id)
        {
            try
            {
                var existingValuation = await _valuationRepository.GetValuationByIdAsync(id);
                if (existingValuation == null)
                {
                    return ResultService<string>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                await _valuationRepository.DeleteValuationAsync(id);
                return ResultService<string>.GoodResult(
                    "Valuation deleted successfully.",
                    StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return ResultService<string>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<IEnumerable<ValuationResponse>>> GetAllValuationsAsync()
        {
            try
            {
                var valuations = await _valuationRepository.GetAllValuationsAsync();
                var response = _mapper.Map<IEnumerable<ValuationResponse>>(valuations);
                return ResultService<IEnumerable<ValuationResponse>>.GoodResult(
                    "Valuations retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<ValuationResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<ValuationResponse>> GetValuationByIdAsync(Guid id)
        {
            try
            {
                var valuation = await _valuationRepository.GetValuationByIdAsync(id);
                if (valuation == null)
                {
                    return ResultService<ValuationResponse>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                var response = _mapper.Map<ValuationResponse>(valuation);
                return ResultService<ValuationResponse>.GoodResult(
                    "Valuation retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<IEnumerable<ValuationResponse>>> GetValuationsByProjectIdAsync(Guid projectId)
        {
            try
            {
                var valuations = await _valuationRepository.GetValuationsByProjectIdAsync(projectId);
                var response = _mapper.Map<IEnumerable<ValuationResponse>>(valuations);
                return ResultService<IEnumerable<ValuationResponse>>.GoodResult(
                    "Valuations for project retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<IEnumerable<ValuationResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<ValuationResponse>> UpdateValuationAsync(UpdateValuationRequest request)
        {
            try
            {
                var existingValuation = await _valuationRepository.GetValuationByIdAsync(request.Id);
                if (existingValuation == null)
                {
                    return ResultService<ValuationResponse>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                _mapper.Map(request, existingValuation);
                existingValuation.LastModified = DateTime.UtcNow;

                var updatedValuation = await _valuationRepository.UpdateValuationAsync(existingValuation);
                var response = _mapper.Map<ValuationResponse>(updatedValuation);

                return ResultService<ValuationResponse>.GoodResult(
                    "Valuation updated successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                return ResultService<ValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}