using Microsoft.AspNetCore.Identity.UI.Services;

namespace webApplication.Services
{
    // Bu sınıf "IEmailSender" arayüzünü taklit eder
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Burası şimdilik boş. Gerçek hayatta burada SMTP kodları olur.
            // Amaç sadece hatayı susturmak ve projeyi çalıştırmak.
            return Task.CompletedTask;
        }
    }
}