using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bookshop.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get; set; }

        public EmailSender(IConfiguration cfg)
        {
            this.SendGridSecret = cfg.GetValue<string>("SendGrid:SecretKey")!;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridSecret);

            var from = new EmailAddress("noreply@geronimo.design", "Ye Olde Bookshop");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to, subject,"", htmlMessage);

            await client.SendEmailAsync(message);
        }
    }
}
