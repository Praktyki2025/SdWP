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

        public async Task<ResultService<ErrorLogResponseDTO>> GetLogToDatabase(
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

                TypeOfLogEnum typeOfLogEnum;

                if (typeOfLog == "Error")
                {
                    typeOfLogEnum = TypeOfLogEnum.Error;
                }
                else if (typeOfLog == "Warning")
                {
                    typeOfLogEnum = TypeOfLogEnum.Warning;
                }
                else if (typeOfLog == "Info")
                {
                    typeOfLogEnum = TypeOfLogEnum.Info;
                }
                else
                {
                    typeOfLogEnum = TypeOfLogEnum.Info;
                }

                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = errorMessage,
                    StackTrace = stackTrace,
                    Source = source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = typeOfLogEnum
                };



                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<ErrorLogResponseDTO>.GoodResult(
                        errorMessage,
                        StatusCodes.Status200OK
                    ));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                var errorLogDTO = new ErrorLogResponseDTO
                {
                    Id = Guid.NewGuid(),
                    Message = e.Message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLogEnum.Error
                };
                return await _errorLogServices.LoggEvent(errorLogDTO)
                    .ContinueWith(_ => ResultService<ErrorLogResponseDTO>.BadResult(
                        e.Message,
                        StatusCodes.Status500InternalServerError
                        ));
            }
        }
    }
}
