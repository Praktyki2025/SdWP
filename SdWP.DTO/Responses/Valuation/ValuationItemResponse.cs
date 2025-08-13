namespace SdWP.DTO.Responses.Valuation
{
    public class ValuationItemResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public float UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }

    }
}
