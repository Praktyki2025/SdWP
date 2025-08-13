using System.ComponentModel.DataAnnotations;

namespace SdWP.DTO.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string PreviousPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
    }
}
