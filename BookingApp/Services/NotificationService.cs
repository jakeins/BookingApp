using BookingApp.Data.Models;
using BookingApp.Helpers;
using BookingApp.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMessageService messageService;
        private readonly IUserService userService;

        public NotificationService(IMessageService messageService, IUserService userService)
        {
            this.messageService = messageService;
            this.userService = userService;
        }

        public async Task ForgetPasswordMail(ApplicationUser user)
        {
            var code = await userService.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"https://yoursite.com?userId={user.Id}&code={code}";
            var body = $"Click to <a href=\"{callbackUrl}\">link</a>, if you want restore your password";
            var message = new Message
            {
                Destination = user.Email,
                Subject = "Forget Password",
                Body = body
            };
            await messageService.SendAsync(message);
        }
    }
}
