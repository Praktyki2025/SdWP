
using SdWP.DTO.Requests.Mailing;
using SdWP.DTO.Responses.Mailing;
using SdWP.Service.Services;

namespace SdWP.Service.IServices
{
    public interface IEmailService
    {
        Task SendTestEmailAsync(string toEmail);
        Task<ResultService<RemindPasswordResponse>> SendPasswordResetEmailAsync(RemindPasswordRequest dto);
    }
}
