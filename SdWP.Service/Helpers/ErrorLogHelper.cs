using SdWP.Data.Repositories;
using SdWP.DTO.Responses;
using SdWP.Data.Models;
using SdWP.Service.IServices;

namespace SdWP.Service.Helpers
{
    public class ErrorLogHelper : IErrorLogHelper
    {
        private readonly ErrorLogRepository _errorLogRepository;
        public ErrorLogHelper(ErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public async Task LoggEvent(ErrorLogResponse dto)
        {
            var errorLog = new ErrorLog
            {
                Id = dto.Id,
                Message = dto.Message,
                TimeStamp = dto.TimeStamp,
                Source = dto.Source,
                StackTrace = dto.StackTrace,
                UserId = dto.UserId,
                TypeOfLog = dto.TypeOfLog.ToString()
            };

             await _errorLogRepository.AddLogAsync(errorLog);
        }
    }
}
