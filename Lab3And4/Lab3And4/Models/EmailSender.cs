using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Lab3And4.Models
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage(new MailAddress("mail@mail.ru", "test"), new MailAddress(email));
            message.Subject = subject;
            message.Body = htmlMessage;
            message.IsBodyHtml = true;
            var client = new SmtpClient("test_smptp");
            client.Credentials = new NetworkCredential("mail@mail.ru", "testPassword");
            client.EnableSsl = true;
            await client.SendMailAsync(message);
        }
    }
}