using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BookingApp.Helpers;

namespace BookingApp.Services
{
    public class MailMessageService : IMessageService
    {
        //TODO: Add mail configuration properties

        /// <summary>
        /// Send email message by using mail configuration 
        /// </summary>
        /// <param name="message">Message data</param>
        /// <returns>Task of sending message by SMTP-client</returns>
        public Task SendAsync(Message message)
        {
            var from = "asptrainee1@gmail.com";
            var pass = "SomeeSite1";

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(from, pass),
                EnableSsl = true
            };

            var mail = new MailMessage(from, message.Destination)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            return client.SendMailAsync(mail);
        }
    }
}
