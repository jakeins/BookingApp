using BookingApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    interface IBookingRepository : IBasicRepositoryAsync<Booking, int>
    {
        Task<double?> OccupancyByResourceAsync(int resourceId);
        Task UpdateAsync(int id, DateTime? startTime, DateTime? endTime, string editUser, string note);
        Task<IEnumerable<Booking>> GetActiveBookingsOfUserFromCurrentTime(ApplicationUser user);
        Task<IEnumerable<Booking>> GetBookingsOfUser(ApplicationUser user);
        Task<IEnumerable<Booking>> GetActiveBookingsOfResourceFromCurrentTime(int resourceId);
        Task<IEnumerable<Booking>> GetBookingsOfResource(int resourceId);
        Task Terminate(int id, string userId);
    }
}
