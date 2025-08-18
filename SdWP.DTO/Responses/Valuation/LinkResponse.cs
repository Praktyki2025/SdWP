namespace SdWP.DTO.Responses.Valuation
{
    public class LinkResponse
    {
        public string Name { get; set; }
        public string LinkUrl { get; set; }
        public string? Description { get; set; }
        public Guid? ValuationId { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
