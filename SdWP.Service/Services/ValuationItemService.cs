using Microsoft.AspNetCore.Http;
using SdWP.Data.IData;
using SdWP.DTO.Requests.Valuation;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.Valuation;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;

namespace SdWP.Service.Services
{
    public class ValuationItemService : IValuationItemService
    {
        private readonly IValuationItemRepository _valuationItemRepository;
        private readonly ICostTypeRepository _costTypeRepository;
        private readonly ICostCategoryRepsoitory _costCategoryRepsoitory;
        private readonly IUserGroupTypeRepository _userGroupTypeRepository;
        private readonly IErrorLogHelper _errorLogServices;

        private string message = string.Empty;

        public ValuationItemService(
            IValuationItemRepository valuationItemRepository,
            ICostTypeRepository costTypeRepository,
            ICostCategoryRepsoitory costCategoryRepsoitory,
            IUserGroupTypeRepository userGroupTypeRepository,
            IErrorLogHelper errorLogServices
            )
        {
            _valuationItemRepository = valuationItemRepository;
            _costTypeRepository = costTypeRepository;
            _costCategoryRepsoitory = costCategoryRepsoitory;
            _userGroupTypeRepository = userGroupTypeRepository;
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<List<ValuationItemResponse>>> GetValuationList()
        {
            try
            {
                var valuationItems = await _valuationItemRepository.GetAllValuationItemsAsync();

                if (valuationItems == null)
                {
                    message = "Valuation item not found";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationItemService/GetValuationList",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<List<ValuationItemResponse>>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
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
            catch (Exception e)
            {
                message = $"Error during show valuation item list: {e.Message} || throw {e.InnerException}";
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
                    .ContinueWith(_ => ResultService<List<ValuationItemResponse>>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<DeleteValuationItemResponse>> DeleteValuationItem(Guid id)
        {
            try
            {
                var valuationItem = await _valuationItemRepository.GetValuationItemByIdAsync(id);

                if (valuationItem == null)
                {
                    message = "Valuation item not found";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationItemService/DeleteValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<DeleteValuationItemResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                await _valuationItemRepository.DeleteValuationItemAsync(id);

                return ResultService<DeleteValuationItemResponse>.GoodResult(
                    "Valuation item deleted successfully.",
                    StatusCodes.Status200OK,
                    new DeleteValuationItemResponse { Id = id });
            }
            catch (Exception e)
            {
                message = $"Error delete vauation item: {e.Message} || throw {e.InnerException}";
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
                    .ContinueWith(_ => ResultService<DeleteValuationItemResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<CreateValuationItemResponse>> CreateValuationItem(CreateValuationItemRequest request)
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
                        Source = "ValuationItemService/CreateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationItemResponse>.BadResult(
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
                        Source = "ValuationItemService/CreateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationItemResponse>.BadResult(
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
                        Source = "ValuationItemService/CreateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationItemResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
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
                    message = "Error drugin add valuation item";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationItemService/CreateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<CreateValuationItemResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<CreateValuationItemResponse>.GoodResult(
                    "Valuation item created successfully.",
                    StatusCodes.Status201Created,
                    valuationItem);
            }
            catch (Exception e)
            {
                message = $"Error: {e.Message} || throw {e.InnerException}";

                Log.Warning(message);

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
                    .ContinueWith(_ => ResultService<CreateValuationItemResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                        ));
            }
        }

        public async Task<ResultService<UpdateValuationItemResponse>> UpdateValuationItem(UpdateValuationItemRequest request)
        {
            try
            {
                var valuationItem = await _valuationItemRepository.GetValuationItemByIdAsync(request.Id);

                if (valuationItem == null)
                {
                    message = "Valuation item not found";

                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationItemService/UpdateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                Guid? costTypeId = null;
                Guid? coastCategoryId = null;
                Guid? userGroupTypeId = null;

                if (!string.IsNullOrEmpty(request.CostTypeName))
                {
                    costTypeId = await _costTypeRepository.GetGuidByName(request.CostTypeName);
                    if (costTypeId == Guid.Empty)
                    {
                        message = "Cost type not found";

                        Log.Error(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Backend",
                            Source = "ValuationItemService/UpdateValuationItem",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Error
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                                ));
                    }
                }

                if (!string.IsNullOrEmpty(request.CostCategoryName))
                {
                    coastCategoryId = await _costCategoryRepsoitory.GetGuidByName(request.CostCategoryName);
                    if (coastCategoryId == Guid.Empty)
                    {
                        message = "Coast category not found";

                        Log.Error(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Backend",
                            Source = "ValuationItemService/UpdateValuationItem",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                                ));
                    }
                }

                if (!string.IsNullOrEmpty(request.UserGroupTypeName))
                {
                    userGroupTypeId = await _userGroupTypeRepository.GetGuidByName(request.UserGroupTypeName);
                    if (userGroupTypeId == Guid.Empty)
                    {
                        message = "User group not found";

                        Log.Warning(message);

                        var errorLogDTO = new ErrorLogResponse
                        {
                            Id = Guid.NewGuid(),
                            Message = message,
                            StackTrace = "Backend",
                            Source = "ValuationItemService/UpdateValuationItem",
                            TimeStamp = DateTime.UtcNow,
                            TypeOfLog = TypeOfLog.Warning
                        };

                        return await _errorLogServices.LoggEvent(errorLogDTO)
                            .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                                message,
                                StatusCodes.Status400BadRequest
                                ));
                    }
                }

                var update = new UpdateValuationItemResponse
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    CostTypeId = costTypeId,
                    CostCategoryID = coastCategoryId,
                    UserGroupTypeId = userGroupTypeId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice,
                    TotalAmount = request.TotalAmount,
                    RecurrencePeriod = request.RecurrencePeriod,
                    RecurrenceUnit = request.RecurrenceUnit,
                };

                var response = await _valuationItemRepository.UpdateValuationItemAsync(update);

                if (response == null)
                {
                    message = "Can't update valuation item";

                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "ValuationItemService/UpdateValuationItem",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Error
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                return ResultService<UpdateValuationItemResponse>.GoodResult(
                    "Valuation item updated successfully.",
                    StatusCodes.Status200OK,
                    update);

            }
            catch (Exception e)
            {
                message = $"Error: {e.Message} || throw: {e.InnerException}";

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
                    .ContinueWith(_ => ResultService<UpdateValuationItemResponse>.BadResult(
                        message,
                        StatusCodes.Status400BadRequest
                        ));
            }
        }
    }
}