using Microsoft.AspNetCore.Identity.UI.Services;

namespace webApplication.Services
{
    public class EmailSender : IEmailSender //We are here, we manipulate Identify package like we have email server
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask;
        }
    }
}