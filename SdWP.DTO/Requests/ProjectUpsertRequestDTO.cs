using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SdWP.DTO.Requests
{
    public class ProjectUpsertRequestDTO
    {
        public Guid? Id { get; set; } // null when creating

        [Required]
        [StringLength(450, ErrorMessage = "Title cannot exceed 450 characters.")]
        public string Title { get; set; }

        [StringLength(1200, ErrorMessage = "Description cannot exceed 1200 characters.")]
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } // null when creating
        public DateTime? LastModified { get; set; } // null when creating
    }
}
