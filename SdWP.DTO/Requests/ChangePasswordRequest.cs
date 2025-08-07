using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests
{
    public class ChangePasswordRequest
    {
        public string PrevPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
