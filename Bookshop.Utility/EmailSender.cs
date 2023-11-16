using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Bookshop.Utility
{
    public class EmailSender : IAppEmailSender
    {
        public string SendGridSecret { get; set; }
        private readonly EmailAddress _noReplyEmail;
        private readonly SendGridClient _client;
        public EmailSender(IConfiguration cfg)
        {
            this.SendGridSecret = cfg.GetValue<string>("SendGrid:SecretKey")!;
            _noReplyEmail = new EmailAddress(SD.NoReplyEmail, SD.EmailName);
            _client = new SendGridClient(SendGridSecret);
        }


        /// <summary>
        /// Forwards parameters to full email sender overload with blank ("") plaintext.
        /// </summary>
        /// <param name="email">Receiving email address.</param>
        /// <param name="subject">Email subject line.</param>
        /// <param name="htmlMessage">HTML encoded message. Do not double encode.</param>
        /// <returns>Task (void)</returns>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await SendEmailAsync(email, subject, "", htmlMessage);
        }


        /// <summary>
        /// Full simple email sender specifying plainText
        /// </summary>
        /// <param name="email">Receiving email address</param>
        /// <param name="subject">Subject line.</param>
        /// <param name="plainText">Plaintext string.</param>
        /// <param name="htmlMessage">HTML encoded string. Do not double encode.</param>
        /// <returns>Task (void)</returns>
        public async Task SendEmailAsync(string email, string subject, string plainText, string htmlMessage)
        {
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(_noReplyEmail, to, subject, plainText, htmlMessage);

            var response = await _client.SendEmailAsync(message);
        }

        /// <summary>
        /// Send an email based on a template created on SendGrid.com
        /// </summary>
        /// <param name="email">Recipient email address</param>
        /// <param name="templateId">Id of email template to use</param>
        /// <param name="dynamicTemplateData">Data to use for variables.</param>
        /// <returns>Task (void)</returns>
        public async Task SendEmailTemplateAsync(string email, string templateId, object dynamicTemplateData)
        {
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleTemplateEmail(_noReplyEmail, to, templateId, dynamicTemplateData);
            var response = await _client.SendEmailAsync(message);
        }
    }

}
