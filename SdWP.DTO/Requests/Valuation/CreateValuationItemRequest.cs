namespace SdWP.DTO.Requests.Valuation
{
 
    public class CreateValuationItemRequest
    {
        public Guid ValuationId { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid CreatorUserId { get; set; }
        public string CostTypeName { get; set; } = string.Empty;
        public string CostCategoryName { get; set; } = string.Empty;
        public string UserGroupTypeName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public float UnitPrice { get; set; }
        public int RecurrencePeriod { get; set; }
        public string? RecurrenceUnit { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}