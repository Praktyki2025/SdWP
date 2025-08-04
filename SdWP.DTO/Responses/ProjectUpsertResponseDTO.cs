using System.ComponentModel.DataAnnotations;
using SdWP.DTO.Requests;

namespace SdWP.DTO.Responses
{
    public class ProjectUpsertResponseDTO
    {
        public Guid? Id { get; set; }
        [Required]
        [StringLength(450, ErrorMessage = "Title cannot exceed 450 characters.")]
        public string Title { get; set; }

        [StringLength(1200, ErrorMessage = "Description cannot exceed 1200 characters.")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool Success { get; set; } = true;
        public string? Message { get; set; } // Optional message for errors or additional info

        public ProjectUpsertRequestDTO MapToRequest()
        {
            return new ProjectUpsertRequestDTO
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                CreatedAt = this.CreatedAt,
                LastModified = this.LastModified
            };
        }
    }
}
