using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mmt.Api.services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _Configuration;

        public MailService(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _Configuration["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("majouleblanc@gmail.com", "youssef");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
