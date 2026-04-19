using System.Net;
using System.Net.Mail;

namespace API.Helpers
{
    public class SmtpEmailService(IConfiguration configuration) : IEmailService
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task SendPasswordResetEmailAsync(string email, string temporaryPassword, CancellationToken cancellationToken)
        {
            string host = _configuration["Email:Smtp:Host"] ?? string.Empty;
            string from = _configuration["Email:Smtp:From"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("SMTP email settings are not configured.");
            }

            int port = int.TryParse(_configuration["Email:Smtp:Port"], out int configuredPort)
                ? configuredPort
                : 587;

            bool enableSsl = !bool.TryParse(_configuration["Email:Smtp:EnableSsl"], out bool configuredEnableSsl)
                || configuredEnableSsl;

            string? username = _configuration["Email:Smtp:Username"];
            string? password = _configuration["Email:Smtp:Password"];

            using SmtpClient smtpClient = new(host, port)
            {
                EnableSsl = enableSsl
            };

            if (!string.IsNullOrWhiteSpace(username))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
            }

            using MailMessage message = new()
            {
                From = new MailAddress(from, "Travelis"),
                Subject = "Travelis temporary password",
                Body = $"""
Hello,

We received a request to reset your Travelis password.

Your temporary password is:
{temporaryPassword}

Please sign in with this temporary password and change it from your profile as soon as possible.

Travelis
""",
                IsBodyHtml = false
            };

            message.To.Add(email);

            cancellationToken.ThrowIfCancellationRequested();
            await smtpClient.SendMailAsync(message);
        }
    }
}
