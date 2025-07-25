using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests
{
    public class ProjectUpsertRequestDTO
    {
        public Guid? Id { get; set; } // null when creating
        public Guid RequestingUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
