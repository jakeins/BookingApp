using BookingApp.Helpers;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Xunit;

namespace BookingAppTests.Services
{
    public class MailMessageServiceTest
    {
        private readonly Mock<ISmtpService> mockSmtpService;
        private readonly Mock<Message> mockMessage;

        public MailMessageServiceTest()
        {
            mockSmtpService = new Mock<ISmtpService>();
            mockMessage = new Mock<Message>();
        }

        [Fact]
        public async void SendMustSendMailAsync()
        {
            mockSmtpService.SetupGet(smtpService => smtpService.Address).Returns("qwerty@mail.com");
            mockMessage.SetupGet(message => message.Subject).Returns("subject");
            mockMessage.SetupGet(message => message.Body).Returns("body");
            mockMessage.SetupGet(message => message.Destination).Returns("to@mail.com");

            MailMessageService messageService = new MailMessageService(mockSmtpService.Object);
            await messageService.SendAsync(mockMessage.Object);

            mockSmtpService.Verify(smtpService => smtpService.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
        }
    }
}
