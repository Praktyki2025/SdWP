using System.ComponentModel.DataAnnotations;

namespace SdWP.DTO.Requests.Mailing
{
    public class RemindPasswordRequest
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
