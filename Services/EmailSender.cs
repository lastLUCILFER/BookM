using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace BookM.Services
{

    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration config)
        {
            _configuration = config;
        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var senderName = _configuration["EmailSettings:SenderName"];


            var Client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };


            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail,senderName),
                Subject = subject,
                Body =  message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            try
            {
                await Client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Email send failed: {ex.Message}");
                throw;
            }finally
            {
                
                Client.Dispose();
                mailMessage.Dispose();
            }
        }
    }
}