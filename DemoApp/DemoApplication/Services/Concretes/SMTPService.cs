using DemoApplication.Contracts.Email;
using DemoApplication.Database.Models;
using DemoApplication.Options;
using DemoApplication.Services.Abstracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DemoApplication.Services.Concretes
{
    public class SMTPService : IEmailService
    {
        private EmailConfigOptions _emailConfig;

        public SMTPService(IOptions<EmailConfigOptions> emailConfigOptions)
        {
            _emailConfig = emailConfigOptions.Value;
        }

        public void Send(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };
            return emailMessage;
        }

        private void Send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect("smtp.gmail.com", 465 , true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("jeyhunah@code.edu.az", "5810171Az");

                    client.Send(emailMessage);
                }
                catch
                {

                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }
    }
}
