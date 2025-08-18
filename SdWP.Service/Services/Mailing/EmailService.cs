using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using SdWP.Data.Models;
using SdWP.DTO.Requests.Mailing;
using SdWP.DTO.Responses;
using SdWP.DTO.Responses.Mailing;
using SdWP.Service.Enums;
using SdWP.Service.IServices;
using Serilog;

namespace SdWP.Service.Services.Mailing
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IErrorLogHelper _errorLogServices;
        private readonly UserManager<User> _userManager;

        private string message = string.Empty;

        public EmailService(
            IOptions<EmailSettings> settings,
            IErrorLogHelper errorLogServices,
            UserManager<User> userManager)
        {
            _settings = settings.Value;
            _errorLogServices = errorLogServices;
            _userManager = userManager;
        }

        public async Task SendTestEmailAsync(string toEmail)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FormName, _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "Test Email from SdWP Service";

                message.Body = new TextPart("plain")
                {
                    Text = "This is a test email sent from the SdWP Service."
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.None);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                message = $"Error sending test email: {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                await _errorLogServices.LoggEvent(errorLogDTO)
                .ContinueWith(_ => ResultService<LoginResponse>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError
                ));
            }
        }

        public async Task<ResultService<RemindPasswordResponse>> SendPasswordResetEmailAsync(RemindPasswordRequest dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);

                if (user == null)
                {
                    message = $"User with this email not found: {dto.Email}";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "EmailService/SendPasswordResetEmailAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<RemindPasswordResponse>
                        .BadResult(
                            message,
                            StatusCodes.Status404NotFound
                        ));
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                if (string.IsNullOrEmpty(token))
                {
                    message = $"Could not generate password reset token";
                    Log.Warning(message);

                    var errorLogDTO = new ErrorLogResponse
                    {
                        Id = Guid.NewGuid(),
                        Message = message,
                        StackTrace = "Unknow",
                        Source = "EmailService/SendPasswordResetEmailAsync",
                        TimeStamp = DateTime.UtcNow,
                        TypeOfLog = TypeOfLog.Warning
                    };

                    return await _errorLogServices.LoggEvent(errorLogDTO)
                        .ContinueWith(_ => ResultService<RemindPasswordResponse>
                        .BadResult(
                            message,
                            StatusCodes.Status400BadRequest
                        ));
                }

                var encodetToken = Uri.EscapeDataString(token);

                var resetLink = $"https://localhost:7019/reset-password?email={dto.Email}&token={encodetToken}";

                var remaindMessage = new MimeMessage();
                remaindMessage.From.Add(new MailboxAddress(_settings.FormName, _settings.FromEmail));
                remaindMessage.To.Add(MailboxAddress.Parse(dto.Email));
                remaindMessage.Subject = "Reset your password";

                var body = new BodyBuilder
                { 
                    HtmlBody = $@"
                        <p>Hello,</p>
                        <p>You requested a password reset. Click the link below to reset your password:</p>
                        <p><a href='{resetLink}'>Reset Password</a></p>
                        <p>If you did not request this, ignore this email.</p>
                        "
                };

                remaindMessage.Body = body.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, MailKit.Security.SecureSocketOptions.None);
                await client.SendAsync(remaindMessage);
                await client.DisconnectAsync(true);

                return ResultService<RemindPasswordResponse>.GoodResult(
                    "Reset link sent successfully",
                    StatusCodes.Status200OK,
                    new RemindPasswordResponse
                    {
                        Email = dto.Email,
                        ResetLink = resetLink
                    }
                );
            }
            catch (Exception e)
            {
                message = $"Error sending remind password email: {e.Message} || throw {e.InnerException}";
                Log.Error(message);

                var errorLogDTO = new ErrorLogResponse
                {
                    Id = Guid.NewGuid(),
                    Message = message,
                    StackTrace = e.StackTrace,
                    Source = e.Source,
                    TimeStamp = DateTime.UtcNow,
                    TypeOfLog = TypeOfLog.Error
                };

                return await _errorLogServices.LoggEvent(errorLogDTO)
                .ContinueWith(_ => ResultService<RemindPasswordResponse>.BadResult(
                    message,
                    StatusCodes.Status500InternalServerError
                ));
            }
        }
    
    }
}
