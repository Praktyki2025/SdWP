using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Requests.Valuation.Name;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.IServices;

namespace SdWP.Service.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IValuationRepository _valuationRepository;
        private readonly ICostTypeRepository _costTypeRepository;
        private readonly ICostCategoryRepsoitory _costCategoryRepsoitory;
        private readonly IUserGroupTypeRepository _userGroupTypeRepository;
        private readonly IProjectRepository _projectRepository;

        public ValuationService(
            IValuationRepository valuationRepository,
            ICostTypeRepository costTypeRepository,
            ICostCategoryRepsoitory costCategoryRepsoitory,
            IUserGroupTypeRepository userGroupTypeRepository,
            IProjectRepository projectRepository
            )
        {
            _valuationRepository = valuationRepository;
            _costTypeRepository = costTypeRepository;
            _costCategoryRepsoitory = costCategoryRepsoitory;
            _userGroupTypeRepository = userGroupTypeRepository;
            _projectRepository = projectRepository;
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
                return ResultService<List<ValuationResponse>>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<CreateValuationResponse>> CreateValuation(CreateValuationRequest request)
        {
            try
            {
                var costTypeId = await _costTypeRepository.GetGuidByName(request.CostTypeName);
                if (costTypeId == Guid.Empty)
                {
                    return ResultService<CreateValuationResponse>.BadResult(
                        "Cost type not found.",
                        StatusCodes.Status404NotFound);
                }

                var coastCategoryId = await _costCategoryRepsoitory.GetGuidByName(request.CostCategoryName);
                if (coastCategoryId == Guid.Empty)
                {
                    return ResultService<CreateValuationResponse>.BadResult(
                        "Cost category not found.",
                        StatusCodes.Status404NotFound);
                }

                var userGroupTypeId = await _userGroupTypeRepository.GetGuidByName(request.UserGroupTypeName);
                if (userGroupTypeId == Guid.Empty)
                {
                    return ResultService<CreateValuationResponse>.BadResult(
                        "User group type not found.",
                        StatusCodes.Status404NotFound);
                }


                var valuation = new CreateValuationResponse
                {
                    ValuationId = Guid.NewGuid(),
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
                    ProjectId = request.ProjectId
                };

                var result = await _valuationRepository.AddValuationAsync(valuation);
                if (result == null)
                {
                    return ResultService<CreateValuationResponse>.BadResult(
                        "Failed to create valuation.",
                        StatusCodes.Status500InternalServerError);
                }

                return ResultService<CreateValuationResponse>.GoodResult(
                    "Valuation created successfully.",
                    StatusCodes.Status201Created,
                    valuation);
            }
            catch (Exception ex)
            {
                return ResultService<CreateValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message} | INNER: {ex.InnerException?.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<CostCategoryNameRequest>> GetCostCategoryName()
        {
            try
            {
                var costCategoryNames = await _costCategoryRepsoitory.GetAllNamesAsync();

                if (costCategoryNames == null || !costCategoryNames.Any())
                {
                    return ResultService<CostCategoryNameRequest>.BadResult(
                        "No cost categories found.",
                        StatusCodes.Status404NotFound);
                }

                return ResultService<CostCategoryNameRequest>.GoodResult(
                    "Cost categories retrieved successfully.",
                    StatusCodes.Status200OK,
                    new CostCategoryNameRequest
                    {
                        CategoryName = costCategoryNames
                    });
            }
            catch (Exception ex)
            {
                return ResultService<CostCategoryNameRequest>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<CostTypeNameRequest>> GetCostTypeName()
        {
            try
            {
                var costTypeNames = await _costTypeRepository.GetAllNamesAsync();
                if (costTypeNames == null || !costTypeNames.Any())
                {
                    return ResultService<CostTypeNameRequest>.BadResult(
                        "No cost types found.",
                        StatusCodes.Status404NotFound);
                }

                return ResultService<CostTypeNameRequest>.GoodResult(
                    "Cost types retrieved successfully.",
                    StatusCodes.Status200OK,
                    new CostTypeNameRequest
                    {
                        CostTypeName = costTypeNames
                    });
            }
            catch (Exception ex)
            {
                return ResultService<CostTypeNameRequest>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<UserGroupNameRequest>> GetUserGroupName()
        {
            try
            {
                var userGroupNames = await _userGroupTypeRepository.GetAllNamesAsync();
                if (userGroupNames == null || !userGroupNames.Any())
                {
                    return ResultService<UserGroupNameRequest>.BadResult(
                        "No user groups found.",
                        StatusCodes.Status404NotFound);
                }

                return ResultService<UserGroupNameRequest>.GoodResult(
                    "User groups retrieved successfully.",
                    StatusCodes.Status200OK,
                    new UserGroupNameRequest
                    {
                        UserGroupName = userGroupNames
                    });
            }
            catch (Exception ex)
            {
                return ResultService<UserGroupNameRequest>.BadResult(
                    $"An error occurred: {ex.Message}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<DeleteValuationResponse>> DeleteValuation(Guid id)
        {
            try
            {
                var exist = await _valuationRepository.GetValuationByIdAsync(id);

                if (exist == null)
                {
                    return ResultService<DeleteValuationResponse>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                await _valuationRepository.DeleteValuationAsync(id);

                return ResultService<DeleteValuationResponse>.GoodResult(
                    "Valuation deleted successfully.",
                    StatusCodes.Status200OK,
                    new DeleteValuationResponse { Id = id });

            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : "";
                return ResultService<DeleteValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message} Inner exception: {innerMessage}",
                    StatusCodes.Status500InternalServerError);
            }
        }

        public async Task<ResultService<UpdateValuationResponse>> UpdateValuation(UpdateValuationRequest request)
        {
            try
            {
                var valuation = await _valuationRepository.GetValuationByIdAsync(request.Id);

                if (valuation == null)
                {
                    return ResultService<UpdateValuationResponse>.BadResult(
                        "Valuation not found.",
                        StatusCodes.Status404NotFound);
                }

                if (valuation.CreatorUserId == null)
                {
                    return ResultService<UpdateValuationResponse>.BadResult(
                        "Creator user ID is null.",
                        StatusCodes.Status400BadRequest);
                }

                if (request.ProjectId == Guid.Empty)
                {
                    return ResultService<UpdateValuationResponse>.BadResult(
                        "Project ID is null.",
                        StatusCodes.Status400BadRequest);
                }

                var projectExists = await _projectRepository.GetByIdAsync(request.ProjectId);
                if (projectExists == null)
                {
                    return ResultService<UpdateValuationResponse>.BadResult(
                        "Project not found.",
                        StatusCodes.Status404NotFound);
                }

                var update = new UpdateValuationResponse
                {
                    Id = request.Id,
                    ProjectId = request.ProjectId,
                    Name = request.Name,
                    Description = request.Description,
                    CreatorUserId = request.CreatorUserId
                };

                var response = await _valuationRepository.UpdateValuationAsync(update);

                if (response == null)
                {
                    return ResultService<UpdateValuationResponse>.BadResult(
                        "Failed to update valuation.",
                        StatusCodes.Status500InternalServerError);
                }

                return ResultService<UpdateValuationResponse>.GoodResult(
                    "Valuation retrieved successfully.",
                    StatusCodes.Status200OK,
                    update);
            }
            catch (Exception ex)
            {
                return ResultService<UpdateValuationResponse>.BadResult(
                    $"An error occurred: {ex.Message} | throw {ex.InnerException}",
                    StatusCodes.Status500InternalServerError);
            }
        }
    }
}