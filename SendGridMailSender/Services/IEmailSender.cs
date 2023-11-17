using SendGrid;

namespace SendGridMailSender.Services
{
    public interface IEmailSender
    {
        Task SendContactUsEmail(string userEmail, string userName, string body);

        Task SendAdminNotification(string filename);
    }
}