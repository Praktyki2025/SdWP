namespace SdWP.DTO.Requests
{
    public class ProjectUpsertResponseDTO
    {
        public Guid? Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool Success { get; set; } = true;
    }
}
