using BookingApp.Data.Models;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class NotificationService
    {
        private readonly MailMessageService mailService;
        private readonly UserManager<ApplicationUser> userManager;

        public NotificationService(MailMessageService mailService, UserManager<ApplicationUser> userManager)
        {
            this.mailService = mailService;
            this.userManager = userManager;
        }

        public async Task ForgetPasswordMail(ApplicationUser user)
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"https://yoursite.com?userId={user.Id}&code={code}";
            var body = $"Click to <a href=\"{callbackUrl}\">link</a>, if you want restore your password";
            var message = new Message
            {
                Destination = user.Email,
                Subject = "Forget Password",
                Body = body
            };
            await mailService.SendAsync(message);
        }
    }
}
