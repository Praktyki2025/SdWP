using SdWP.DTO.Responses;
using SdWP.Service.Enums;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IErrorLogServices
    {
        Task<ResultService<ErrorLogResponseDTO>> GetLogToDatabase(
            string? errorMessage,
            string? stackTrace,
            string? source,
            string? typeOfLog);
    }
}
