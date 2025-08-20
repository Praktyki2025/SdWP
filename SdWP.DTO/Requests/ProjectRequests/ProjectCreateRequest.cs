using System.ComponentModel.DataAnnotations;

namespace SdWP.DTO.Requests.ProjectRequests
{
    public class ProjectCreateRequest
    {
        [Required]
        [StringLength(450, ErrorMessage = "Title cannot exceed 450 characters.")]
        public string Title { get; set; }

        [StringLength(1200, ErrorMessage = "Description cannot exceed 1200 characters.")]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
