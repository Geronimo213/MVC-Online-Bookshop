using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bookshop.Utility
{
    public interface IAppEmailSender : IEmailSender
    {
        /// <summary>
        /// Sends simple email with provided parameters.
        /// </summary>
        /// <param name="email">Receiving email address</param>
        /// <param name="subject">Email subject</param>
        /// <param name="plainText">Plain text message string.</param>
        /// <param name="htmlMessage">HTML message string. Do not double encode.</param>
        /// <returns>Task (void)</returns>
        Task SendEmailAsync(string email, string subject, string plainText, string htmlMessage);

        /// <summary>
        /// Sends email using provided template id. Templates are stored on SendGrid.com
        /// </summary>
        /// <param name="email">Recipient email</param>
        /// <param name="templateId">Id for the template to use.</param>
        /// <param name="dynamicTemplateData">data</param>
        /// <returns></returns>
        Task SendEmailTemplateAsync(string email, string templateId, object dynamicTemplateData);
    }
}
