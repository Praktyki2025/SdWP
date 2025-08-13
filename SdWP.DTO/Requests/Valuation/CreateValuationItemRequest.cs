namespace SdWP.DTO.Requests.Valuation
{
 
    public class CreateValuationItemRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Guid CreatorUserId { get; set; }
        public Guid CostTypeId { get; set; }
        public Guid CostCategoryID { get; set; }
        public Guid UserGroupTypeId { get; set; }
        public decimal Quantity { get; set; }
        public float UnitPrice { get; set; }
        public int RecurrencePeriod { get; set; }
        public string? RecurrenceUnit { get; set; }
    }
}