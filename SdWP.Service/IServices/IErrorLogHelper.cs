using SdWP.DTO.Enums;
using SdWP.DTO.Responses;
using SdWP.Service.Services; 
using SdWP.Data.Models;
namespace SdWP.Service.IServices
{
    public interface IErrorLogHelper
    {
        Task LoggEvent(ErrorLogResponse dto);
    }
}
