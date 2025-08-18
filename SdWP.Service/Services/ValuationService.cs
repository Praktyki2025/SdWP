using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Requests.Valuation.Name;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;

namespace SdWP.Service.Services
{
    public class ValuationService : IValuationService
    {
        private readonly IValuationRepository _valuationRepository;
        private readonly ICostTypeRepository _costTypeRepository;
        private readonly ICostCategoryRepsoitory _costCategoryRepsoitory;
        private readonly IUserGroupTypeRepository _userGroupTypeRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IErrorLogHelper _errorLogServices;

        private string message = string.Empty;

        public ValuationService(
            IValuationRepository valuationRepository,
            ICostTypeRepository costTypeRepository,
            ICostCategoryRepsoitory costCategoryRepsoitory,
            IUserGroupTypeRepository userGroupTypeRepository,
            IProjectRepository projectRepository,
            IErrorLogHelper errorLogServices
            )
        {
            _valuationRepository = valuationRepository;
            _costTypeRepository = costTypeRepository;
            _costCategoryRepsoitory = costCategoryRepsoitory;
            _userGroupTypeRepository = userGroupTypeRepository;
            _projectRepository = projectRepository;
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<List<ValuationResponse>>> GetValuationList()
        {
            try
            {
                var valuation = await _valuationRepository.GetAllValuationsAsync();

                if (valuation == null)
                {
                    message = "Valuation not found";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/GetValuationList",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<List<ValuationResponse>>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
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
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<List<ValuationResponse>>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<CreateValuationResponse>> CreateValuation(CreateValuationRequest request)
        {
            try
            {
                var costTypeId = await _costTypeRepository.GetGuidByName(request.CostTypeName);
                if (costTypeId == Guid.Empty)
                {
                    message = "Cost type not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/CreateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                var coastCategoryId = await _costCategoryRepsoitory.GetGuidByName(request.CostCategoryName);
                if (coastCategoryId == Guid.Empty)
                {
                    message = "Cost category not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/CreateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                var userGroupTypeId = await _userGroupTypeRepository.GetGuidByName(request.UserGroupTypeName);
                if (userGroupTypeId == Guid.Empty)
                {
                    message = "User group type not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/CreateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
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
                    message = "Error during add valuation";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/CreateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<CreateValuationResponse>.GoodResult(
                    "Valuation created successfully.",
                    StatusCodes.Status201Created,
                    valuation);
            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<CreateValuationResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<CostCategoryNameRequest>> GetCostCategoryName()
        {
            try
            {
                var costCategoryNames = await _costCategoryRepsoitory.GetAllNamesAsync();

                if (costCategoryNames == null || !costCategoryNames.Any())
                {
                    message = "No cost category found";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/GetCostCategoryName",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CostCategoryNameRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<CostCategoryNameRequest>.GoodResult(
                    "Cost categories retrieved successfully.",
                    StatusCodes.Status200OK,
                    new CostCategoryNameRequest
                    {
                        CategoryName = costCategoryNames
                    });
            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<CostCategoryNameRequest>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<CostTypeNameRequest>> GetCostTypeName()
        {
            try
            {
                var costTypeNames = await _costTypeRepository.GetAllNamesAsync();
                if (costTypeNames == null || !costTypeNames.Any())
                {
                    message = "No cost type name found";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/GetCostTypeName",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CostTypeNameRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<CostTypeNameRequest>.GoodResult(
                    "Cost types retrieved successfully.",
                    StatusCodes.Status200OK,
                    new CostTypeNameRequest
                    {
                        CostTypeName = costTypeNames
                    });
            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<CostTypeNameRequest>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<UserGroupNameRequest>> GetUserGroupName()
        {
            try
            {
                var userGroupNames = await _userGroupTypeRepository.GetAllNamesAsync();
                if (userGroupNames == null || !userGroupNames.Any())
                {
                    message = "No user group name found";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/CreateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UserGroupNameRequest>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<UserGroupNameRequest>.GoodResult(
                    "User groups retrieved successfully.",
                    StatusCodes.Status200OK,
                    new UserGroupNameRequest
                    {
                        UserGroupName = userGroupNames
                    });
            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UserGroupNameRequest>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<DeleteValuationResponse>> DeleteValuation(Guid id)
        {
            try
            {
                var exist = await _valuationRepository.GetValuationByIdAsync(id);

                if (exist == null)
                {
                    message = "Cost type not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/DeleteValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<DeleteValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                await _valuationRepository.DeleteValuationAsync(id);

                return ResultService<DeleteValuationResponse>.GoodResult(
                    "Valuation deleted successfully.",
                    StatusCodes.Status200OK,
                    new DeleteValuationResponse { Id = id });

            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<DeleteValuationResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<UpdateValuationResponse>> UpdateValuation(UpdateValuationRequest request)
        {
            try
            {
                var valuation = await _valuationRepository.GetValuationByIdAsync(request.Id);

                if (valuation == null)
                {
                    message = "Valuation not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/UpdateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                if (valuation.CreatorUserId == null)
                {
                    message = "Crator user not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/UpdateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                if (request.ProjectId == Guid.Empty)
                {
                    message = "Project ID not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/UpdateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                var projectExists = await _projectRepository.GetByIdAsync(request.ProjectId);
                if (projectExists == null)
                {
                    message = "Project not found";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/UpdateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
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
                    message = "Error during update valuation";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationService/UpdateValuation",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<UpdateValuationResponse>.GoodResult(
                    "Valuation retrieved successfully.",
                    StatusCodes.Status200OK,
                    update);
            }
            catch (Exception e)
            {
                message = $"Error : {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<UpdateValuationResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }
    }
}