using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Requests.Valuation
{
    public class CreateValuationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatorUserId { get; set; }
        public string CostTypeName { get; set; } = string.Empty;
        public string CostCategoryName { get; set; } = string.Empty;
        public string UserGroupTypeName { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public float? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? RecurrencePeriod { get; set; }
        public string? RecurrenceUnit { get; set; }
        public Guid ProjectId { get; set; }
    }
}
