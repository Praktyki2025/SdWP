namespace SdWP.DTO.Requests.Valuation
{
    public class UpdateLinkRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? LinkUrl { get; set; }
        public string? Description { get; set; }
    }
}
