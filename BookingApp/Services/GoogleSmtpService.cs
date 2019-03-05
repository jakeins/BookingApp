using BookingApp.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class GoogleSmtpService : ISmtpService
    {
        private readonly SmtpClient client;

        public GoogleSmtpService()
        {
            var pass = "SomeeSite1";

            client = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(Address, pass),
                EnableSsl = true
            };
        }

        public string Address => "asptrainee1@gmail.com";

        public Task SendMailAsync(MailMessage message)
        {
            return client.SendMailAsync(message);
        }
    }
}
