using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using BookingApp.Helpers;
using BookingApp.Services.Interfaces;

namespace BookingApp.Services
{
    public class MailMessageService : IMessageService
    {
        private readonly ISmtpService smtpService;

        public MailMessageService(ISmtpService smtpService)
        {
            this.smtpService = smtpService;
        }

        /// <summary>
        /// Send email message by using mail configuration 
        /// </summary>
        /// <param name="message">Message data</param>
        /// <returns>Task of sending message by SMTP-client</returns>
        public Task SendAsync(Message message)
        {
            var mail = new MailMessage(smtpService.Address, message.Destination)
            {
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };

            return smtpService.SendMailAsync(mail);
        }
    }
}
