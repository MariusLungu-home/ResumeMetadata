using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SendGridMailSender.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _apiKey = "";
        private readonly string _adminFromEmail = "";
        private readonly string _notificationEmail = "";
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _apiKey = Environment.GetEnvironmentVariable("sendgridApiKey");
            _adminFromEmail = Environment.GetEnvironmentVariable("adminFromEmail");
            _notificationEmail = Environment.GetEnvironmentVariable("notificationEmail1");
            _logger = logger;
        }
        public async Task SendContactUsEmail(string userEmail, string userName, string body)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_adminFromEmail);
                var to = new EmailAddress(_notificationEmail);
                var subject = $"Contact Us Request from {userName}";
                var plainTextContent = $"{userEmail} said:\n{body}";
                var htmlContent = $"{userEmail} said <p>{body}</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: contact us method. {ex.Message}");
            }
        }

        public async Task SendAdminNotification(string filename)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_adminFromEmail);
                var subject = "New File Upload Notification";
                var to = new EmailAddress(_notificationEmail);
                var plainTextContent = $"Filename {filename} has been uploaded.";
                var htmlContent = $"Filename: <strong>{filename}</strong> has been uploaded.";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: admin info notification method. {ex.Message}");
            }
        }
    }
}