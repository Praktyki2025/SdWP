using SdWP.DTO.Requests.ProjectRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdWP.DTO.Responses.ProjectRequests
{
    public class ProjectResponse
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(450, ErrorMessage = "Title cannot exceed 450 characters.")]
        public required string Title { get; set; }

        [StringLength(1200, ErrorMessage = "Description cannot exceed 1200 characters.")]
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }

        public ProjectEditRequest MapToRequest()
        {
            return new ProjectEditRequest
            {
                Id = Id,
                Title = Title,
                Description = Description,
                LastModified = LastModified
            };
        }
    }
}
