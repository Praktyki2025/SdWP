using System.ComponentModel.DataAnnotations;

namespace SdWP.DTO.Requests
{
    public class EditUserRequest
    {
        public Guid Id { get; set; }

        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string? Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; } = string.Empty;

        public string? Password { get; set; } = string.Empty;

        public string? ConfirmPassword { get; set; } = string.Empty;

        public string? Role { get; set; } = string.Empty;

        public DateTime LastUpdate { get; set; }
    }
}
