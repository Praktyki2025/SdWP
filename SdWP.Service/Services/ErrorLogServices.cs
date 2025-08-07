using Microsoft.AspNetCore.Http;
using SdWP.DTO.Responses;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;

namespace SdWP.Service.Services
{
    public class ErrorLogServices : IErrorLogServices
    {
        private readonly IErrorLogHelper _errorLogServices;
        public ErrorLogServices(IErrorLogHelper errorLogServices)
        {
            _errorLogServices = errorLogServices;
        }

        public async Task<ResultService<ErrorLogResponse>> GetLogToDatabase(
            string? errorMessage,
            string? stackTrace,
            string? source,
            string typeOfLog)
        {
            try
            {
                if (string.IsNullOrEmpty(errorMessage)) errorMessage = "No error message provided.";
                if (string.IsNullOrEmpty(stackTrace)) stackTrace = "No stack trace provided.";
                if (string.IsNullOrEmpty(source)) source = "No source provided.";

                TypeOfLog typeOfLogEnum;

                if (typeOfLog == "Error")
                {
                    typeOfLogEnum = TypeOfLog.Error;
                }
                else if (typeOfLog == "Warning")
                {
                    typeOfLogEnum = TypeOfLog.Warning;
                }
                else if (typeOfLog == "Info")
                {
                    typeOfLogEnum = TypeOfLog.Info;
                }
                else
                {
                    typeOfLogEnum = TypeOfLog.Info;
                }

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = errorMessage,
                    StackTrace = stackTrace,
                    Source = source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = typeOfLogEnum
                };



                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<ErrorLogResponse>.GoodResult(
                        errorMessage,
                        StatusCodes.Status200OK
                    ));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };
                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<ErrorLogResponse>.BadResult(
                        e.Message,
                        StatusCodes.Status500InternalServerError
                        ));
            }
        }
    }
}
