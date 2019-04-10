using BookingApp.Data.Models;
using BookingApp.Helpers;
using BookingApp.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMessageService messageService;
        public IConfiguration Configuration { get; }

        public NotificationService(IMessageService messageService, IConfiguration configuration)
        {
            this.messageService = messageService;
            Configuration = configuration;
        }

        public async Task SendPasswordResetNotification(ApplicationUser user, string passResetToken)
        {
            await messageService.SendAsync(new Message
            {
                Destination = user.Email,
                Subject = "BookingApp New Password",
                Body = $"<h1>Hello {user.UserName}</h1>" +
                $"<p>Please proceed by the following <a href=\"https://{Configuration["HostingDomain"]}/reset?userId={user.Id}&code={passResetToken}\">link</a> to set your new password.</p>" +
                "<p>Happy booking!</p>"
            });
        }

        public async Task SendApprovalNotification(ApplicationUser user)
        {
            await messageService.SendAsync(new Message
            {
                Destination = user.Email,
                Subject = "BookingApp Account",
                Body = $"<h1>Hello {user.UserName}</h1>" +
                $"<p>Your account {user.Email} is ready for usage.</p>" +
                $"Having your password, <a href=\"https://{Configuration["HostingDomain"]}/login\">sign in</a> to use <b>Booking App</b>.</p>" +
                "<p>Happy booking!</p>"
            });
        }
    }
}
