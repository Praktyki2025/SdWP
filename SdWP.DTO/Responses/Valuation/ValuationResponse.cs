using System;
using System.Collections.Generic;

namespace SdWP.DTO.Responses.Valuation
{
    public class ValuationResponse
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
    }
}
