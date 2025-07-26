using System;
using System.Collections.Generic;
namespace SdWP.DTO.Requests
{
    public class ProjectUpsertRequestDTO
    {
        public Guid? Id { get; set; } // null when creating
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; } // null when creating
        public DateTime? LastModified { get; set; } // null when creating
    }
}
