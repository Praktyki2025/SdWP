using SdWP.Service.Enums;

namespace SdWP.DTO.Requests
{
    public class ErrorLogRequest
    {
        public string Message { get; set; } = string.Empty; 
        public string StackTrace { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public TypeOfLog TypeOfLog { get; set; }
    }
}
