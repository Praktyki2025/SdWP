using SdWP.DTO.Enums;
using SdWP.DTO.Responses;
using SdWP.Service.Services; 
using SdWP.Data.Models;
namespace SdWP.Service.IServices
{
    public interface IErrorLogServices
    {
        Task LoggEvent(ErrorLogResponseDTO dto);
    }
}
