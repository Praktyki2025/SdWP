namespace SdWP.DTO.Responses.Valuation
{
    public class UpdateValuationResponse
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid CreatorUserId { get; set; }
    }
}
