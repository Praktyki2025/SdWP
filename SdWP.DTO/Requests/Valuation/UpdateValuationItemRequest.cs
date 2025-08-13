namespace SdWP.DTO.Requests.Valuation
{
    public class UpdateValuationItemRequest
    {
        public Guid Id { get; set; }
        public Guid CreatedUserId { get; set; }
        public Guid ValuationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? CostTypeName { get; set; }
        public string? CostCategoryName { get; set; }
        public string? UserGroupTypeName { get; set; }
        public decimal? Quantity { get; set; }
        public float? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? RecurrencePeriod { get; set; }
        public string? RecurrenceUnit { get; set; }
    }
}
