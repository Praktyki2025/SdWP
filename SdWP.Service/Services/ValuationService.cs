using SdWP.Data.IData;
using SdWP.DTO.Requests;
using SdWP.DTO.Responses;
using Microsoft.AspNetCore.Http;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IValuationRepository _valuationRepository;

        public ValuationService(IValuationRepository valuationRepository)
        {
            _valuationRepository = valuationRepository;
        }

        public async Task<ResultService<List<ValuationResponse>>> GetValuationList()
        {
            try
            {
                var valuation = await _valuationRepository.GetAllValuationsAsync();

                if (valuation == null)
                {
                    return ResultService<List<ValuationResponse>>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                var response = valuation.Select(v => new ValuationResponse
                {
                    Id = v.Id,
                    Name = v.Name,
                    ProjectId = v.ProjectId,
                    Description = v.Description,
                    CreatedAt = v.CreatedAt,
                    LastModified = v.LastModified
                }).ToList();

                return ResultService<List<ValuationResponse>>.GoodResult(
                    "Valuation retrieved successfully.",
                    StatusCodes.Status200OK,
                    response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error chuj: {ex.ToString()}");
                return ResultService<List<ValuationResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}