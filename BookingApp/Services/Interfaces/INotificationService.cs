using BookingApp.Data.Models;
using System.Threading.Tasks;

namespace BookingApp.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendPasswordResetNotification(ApplicationUser user, string passResetToken);
        Task SendApprovalNotification(ApplicationUser user);
    }
}
