using BookingApp.Data.Models;
using BookingApp.Helpers;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace BookingAppTests.Services
{
    public class NotificationServicesTest
    {
        private readonly Mock<IConfiguration> mockConfigService;
        private readonly Mock<IMessageService> mockMessageService;
        private readonly Mock<IUserService> mockUserService;
        private readonly Mock<ApplicationUser> mockUser;

        public NotificationServicesTest()
        {
            mockMessageService = new Mock<IMessageService>();
            mockUserService = new Mock<IUserService>();
            mockUser = new Mock<ApplicationUser>();
            mockConfigService = new Mock<IConfiguration>();
        }

        [Theory]
        [InlineData("token1")]
        [InlineData("token2")]
        [InlineData("token3")]
        public async void ForgetPasswordMailMustSendForgetMessageForUser(string resetToken)
        {
            var body = $"Click to <a href=\"https://yoursite.com?userId={mockUser.Object.Id}&code={resetToken}\">link</a>, if you want restore your password";
            var message = new Message
            {
                Body = body,
                Destination = mockUser.Object.Email,
                Subject = "Forget Password"
            };
            mockUserService.Setup(userService => userService.GeneratePasswordResetTokenAsync(mockUser.Object)).ReturnsAsync(resetToken);


            NotificationService notificationService = new NotificationService(mockMessageService.Object, mockConfigService.Object);
            await notificationService.SendPasswordResetNotification(mockUser.Object, "token");

            mockMessageService.Verify(messageService => messageService.SendAsync(It.IsAny<Message>()), Times.Once);
        }
    }
}
