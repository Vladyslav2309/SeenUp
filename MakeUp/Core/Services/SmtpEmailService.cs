using Core.Interfaces;
using Core.Models.Account;
using MailKit.Net.Smtp;
using MimeKit;

namespace Core.Services
{
    public class SmtpEmailService : ISmtpEmailService
    {
        private readonly EmailConfiguration _configuration;
        public SmtpEmailService()
        {
            _configuration = new EmailConfiguration()
            {
                From = "super.novakvova@ukr.net",
                SmtpServer = "smtp.ukr.net",
                Port = 2525,
                UserName = "super.novakvova@ukr.net",
                Password = "MVjWbajb9tGqiHgK"
            };
        }

        public void Send(Message message)
        {
            var builder = new BodyBuilder();
            builder.HtmlBody = message.Body;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_configuration.From, _configuration.From));
            emailMessage.To.Add(new MailboxAddress(message.To, message.To));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.SmtpServer, _configuration.Port, true);
                    client.Authenticate(_configuration.UserName, _configuration.Password);
                    client.Send(emailMessage);
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Send message error {0}", ex.Message);
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
