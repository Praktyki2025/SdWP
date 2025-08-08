using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Responses
{
    public  class UserListResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public List<string> Roles { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Success { get; set; }
    }
}
