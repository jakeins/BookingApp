using BookingApp.Data.Models;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public class BookingsService
    {
        readonly BookingsRepository bookingsRepo;

        public BookingsService(BookingsRepository bookingRepository)
        {
            bookingsRepo = bookingRepository;
        }

        #region CRUD operations

        public async Task<IEnumerable<Booking>> ListBookingForSpecificUser(ApplicationUser user, bool asAdmin)
        {
            if (asAdmin)
                return await bookingsRepo.GetBookingsOfUser(user);
            else
                return await bookingsRepo.GetActiveBookingsOfUserFromCurrentTime(user);
        }

        public async Task<IEnumerable<Booking>> ListBookingOfResource(int resourceId, bool asAdmin)
        {
            if (asAdmin)
                return await bookingsRepo.GetBookingsOfResource(resourceId);
            else
                return await bookingsRepo.GetActiveBookingsOfResourceFromCurrentTime(resourceId);
        }

        public async Task Create(Resource resource, DateTime startTime, DateTime endTime, ApplicationUser createdBy, string note)
        {
            await bookingsRepo.CreateAsync(resource, startTime, endTime, createdBy, note);
        }

        public async Task Update(int id, DateTime startTime, DateTime endTime, ApplicationUser editUser, string note)
        {
            await bookingsRepo.UpdateAsync(id, startTime, endTime, editUser, note);
        }

        public async Task Delete(int id)
        {
            await bookingsRepo.DeleteAsync(id);
        }

        #endregion
    }
}
