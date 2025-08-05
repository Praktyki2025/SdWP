using SdWP.Service.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Responses
{
    public class ErrorLogResponseDTO
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public DateTime TimeStamp { get; set; }
        public Guid? UserId { get; set; }

       public TypeOfLogEnum TypeOfLog { get; set; }
    }
}
