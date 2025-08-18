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
    public class LinkServices : ILinkServices
    {
        private readonly ILinkRepository _linkRepository;
        private readonly IValuationRepository _valuationRepository;
        private readonly IErrorLogHelper _errorLogServices;
        private readonly IProjectRepository _projectRepository;

        private string message = string.Empty;

        public LinkServices(
            ILinkRepository linkRepository,
            IValuationRepository valuationRepository,
            IErrorLogHelper errorLogServices,
            IProjectRepository projectRepository
        )
        {
            _linkRepository = linkRepository;
            _valuationRepository = valuationRepository;
            _errorLogServices = errorLogServices;
            _projectRepository = projectRepository;
        }


        public async Task<ResultService<LinkResponse>> AddLinkAsync(AddLinkRequest request)
        {
            try
            {
                var valuation = await _valuationRepository.GetValuationByIdAsync(request.ValuationId);

                if (valuation == null)
                {
                    message = "Valuation not found.";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/AddLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                var project = await _projectRepository.GetByIdAsync(valuation.ProjectId);
                if (project == null)
                {
                    message = "Project not found.";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/AddLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.LinkUrl))
                {
                    message = "Name and LinkUrl are required.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/AddLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                            ));
                }

                var link = new LinkResponse
                {
                    Name = request.Name,
                    LinkUrl = request.LinkUrl,
                    Description = request.Description,
                    ValuationId = request.ValuationId,
                    ProjectId = valuation.ProjectId
                };

                var result = await _linkRepository.AddLinkAsync(link);

                if (result == null)
                {
                    message = "Failed to add link.";
                    Log.Error(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/AddLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<LinkResponse>.BadResult(
                            message,
                            StatusCodes.Status500InternalServerError
                            ));
                }

                return ResultService<LinkResponse>.GoodResult(
                    "Link added successfully.",
                    StatusCodes.Status201Created,
                    link
                );
            }
            catch (Exception e)
            {
                message = $"Error during add link: {e.Message}";
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
                    .ContinueWith(_ => ResultService<LinkResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<List<LinkResponse>>> GetLinkByProjectId(Guid projectId)
        {
            try
            {
                var project = await _projectRepository.GetByIdAsync(projectId);

                if (project == null)
                {
                    message = "Project not found.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/GetLinkByProjectId",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<List<LinkResponse>>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                var link = await _linkRepository.GetAllLinksToProject(projectId);

                if (link == null || !link.Any())
                {
                    message = "Links not found.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/GetLinkByProjectId",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<List<LinkResponse>>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                var response = link.Select(l => new LinkResponse
                {
                    Name = l.Name,
                    LinkUrl = l.LinkUrl,
                    Description = l.Description,
                    ValuationId = l.ValuationId,
                    ProjectId = l.ProjectId,
                }).ToList();

                return ResultService<List<LinkResponse>>.GoodResult(
                        "Valuation retrieved successfully.",
                        StatusCodes.Status200OK,
                        response);
            }
            catch (Exception e)
            {
                message = $"Error during get link by project id: {e.Message}";
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
                    .ContinueWith(_ => ResultService<List<LinkResponse>>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<bool>> DeleteLinkAsync(Guid id)
        {
            try
            {
                var link = await _linkRepository.FindLinkByIdAsync(id);

                if (!link)
                {
                    message = "Link not found.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/DeleteLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<bool>.BadResult(
                            message,
                            StatusCodes.Status404NotFound
                        ));
                }

                var result = await _linkRepository.DeleteLinkAsync(id);

                if (result)
                {
                    return ResultService<bool>.GoodResult(
                        "Link deleted successfully.",
                        StatusCodes.Status200OK,
                        true
                    );
                }
                else
                {
                    message = "Failed to delete link.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/DeleteLinkAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<bool>.BadResult(
                            message,
                            StatusCodes.Status404NotFound
                        ));
                }
            }
            catch (Exception e)
            {
                message = $"Error during delete link: {e.Message}";
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
                    .ContinueWith(_ => ResultService<bool>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                    ));
            }
        }

        public async Task<ResultService<UpdateLinkResponse>> UpdateLinkAsync(UpdateLinkRequest request)
        {
            try
            {
                if (request.Id == null)
                {
                    message = "Link Id not found.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/UpdateLink",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateLinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                var exist = await _linkRepository.FindLinkByIdAsync(request.Id);

                if (exist == null)
                {
                    message = "Link not found.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/UpdateLink",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateLinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                var link = new UpdateLinkResponse
                {
                    Id = request.Id,
                    Name = request.Name,
                    Description = request.Description,
                    LinkUrl = request.LinkUrl
                };

                var result = await _linkRepository.UpdateLinkAsync(link);

                if (result == null)
                {
                    message = "Can't update link info.";
                    Log.Warning(message);
                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Backend",
                        Source = "LinkServices/UpdateLink",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning,
                    };

                    await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<UpdateLinkResponse>.BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                return ResultService<UpdateLinkResponse>.GoodResult(
                    "Link update Successfull",
                    StatusCodes.Status200OK,
                    link
                    );
            }
            catch (Exception e)
            {
                message = $"Error during update link: {e.Message} || throw {e.InnerException}";
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
                    .ContinueWith(_ => ResultService<UpdateLinkResponse>.BadResult(
                        message,
                        StatusCodes.Status500InternalServerError
                    ));
            }
        }
    }
}
