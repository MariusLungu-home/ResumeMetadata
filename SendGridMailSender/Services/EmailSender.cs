using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SendGridMailSender.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _apiKey = "";
        private readonly string _adminFromEmail = "";
        private readonly string _notificationEmail1 = "";
        private readonly string _notificationEmail2 = "";
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger)
        {
            _apiKey = Environment.GetEnvironmentVariable("sendgridApiKey");
            _adminFromEmail = Environment.GetEnvironmentVariable("adminFromEmail");
            _notificationEmail1 = Environment.GetEnvironmentVariable("notificationEmail1");
            _notificationEmail2 = Environment.GetEnvironmentVariable("notificationEmail2");

            _logger = logger;
        }
        public async Task SendContactUsEmail(string userEmail, string userName, string body)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_adminFromEmail);
                var to = new List<EmailAddress>() {new EmailAddress(_notificationEmail1),new EmailAddress(_notificationEmail2)};
                var subject = $"Contact Us Request from {userName}";
                var plainTextContent = $"{userEmail} said:\n{body}";
                var htmlContent = $"{userEmail} said <p>{body}</p>";
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, plainTextContent, htmlContent);

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
                var to = new List<EmailAddress>() { new EmailAddress(_notificationEmail1), new EmailAddress(_notificationEmail2) };
                var plainTextContent = $"Filename {filename} has been uploaded.";
                var htmlContent = $"Filename: <strong>{filename}</strong> has been uploaded.";
                var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, to, subject, plainTextContent, htmlContent);

                await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email: admin info notification method. {ex.Message}");
            }
        }
    }
}