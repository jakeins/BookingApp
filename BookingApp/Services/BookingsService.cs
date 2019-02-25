using BookingApp.Data.Models;
using BookingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Services
{
    public interface IBookingsService
    {
        /// <summary>
        /// Create new <see cref="Booking"></see>
        /// </summary>
        /// <param name="booking">New <see cref="Booking"></see> data</param>
        /// <param name="user">User who create booking</param>
        /// <returns>Id of <see cref="Booking"/></returns>
        Task CreateAsync(Booking model);

        /// <summary>
        /// Delete exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <returns></returns>
        Task Delete(int id);

        /// <summary>
        /// Get <see cref="Booking"></see> by Id
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <returns><see cref="Booking"></see></returns>
        Task<Booking> GetAsync(int id);

        /// <summary>
        /// Return list <see cref="Booking"></see> created by specific <see cref="ApplicationUser"></see>
        /// </summary>
        /// <param name="userId">Id of specific <see cref="ApplicationUser"></see></param>
        /// <param name="startTime">Optional startTime of bookings</param>
        /// <param name="endTime">Optional endTime of bookings</param>
        /// <returns>List of <see cref="Booking"></see></returns>
        Task<IEnumerable<Booking>> ListBookingForSpecificUser(string userId, DateTime? startTime, DateTime? endTime);

        /// <summary>
        /// Return list <see cref="Booking"></see> of specific  <see cref="Resource"></see>
        /// </summary>
        /// <param name="resourceId"><see cref="Booking.ResourceId"></see></param>
        /// <param name="asAdmin">If <see cref="true"></see> than return all bookings of specific <see cref="Resource"></see> else only booking from active in current time or in future</param>
        /// <returns></returns>
        Task<IEnumerable<Booking>> ListBookingOfResource(int resourceId, bool asAdmin);

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <param name="user">ID of <see cref="ApplicationUser"> which terminate booking</see></param>
        /// <returns></returns>
        Task<double?> OccupancyByResource(int resourceId);

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <param name="user">ID of <see cref="ApplicationUser"> which terminate booking</see></param>
        /// <returns></returns>
        Task Terminate(int id, string userId);

        /// <summary>
        /// Update exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see></param>
        /// <param name="startTime">Optional <see cref="Booking.StartTime"></see></param>
        /// <param name="endTime">Optional <see cref="Booking.EndTime"></see></param>
        /// <param name="editUser">User id who edit resource</param>
        /// <param name="note">Optional <see cref="Booking.Note"></see></param>
        /// <returns></returns>
        Task Update(int id, DateTime? startTime, DateTime? endTime, string editUser, string note);
    }

    public class BookingsService : IBookingsService
    {
        private readonly IBookingsRepository bookingsRepo;

        public BookingsService(IBookingsRepository bookingRepository)
        {
            bookingsRepo = bookingRepository;
        }

        #region CRUD operations

        /// <summary>
        /// Create new <see cref="Booking"></see>
        /// </summary>
        /// <param name="booking">New <see cref="Booking"></see> data</param>
        /// <param name="user">User who create booking</param>
        /// <returns>Id of <see cref="Booking"/></returns>
        public async Task CreateAsync(Booking model)
        {
            await bookingsRepo.CreateAsync(model);
        }

        /// <summary>
        /// Get <see cref="Booking"></see> by Id
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <returns><see cref="Booking"></see></returns>
        public async Task<Booking> GetAsync(int id) => await bookingsRepo.GetAsync(id);

        /// <summary>
        /// Return list <see cref="Booking"></see> created by specific <see cref="ApplicationUser"></see>
        /// </summary>
        /// <param name="userId">Id of specific <see cref="ApplicationUser"></see></param>
        /// <param name="startTime">Optional startTime of bookings</param>
        /// <param name="endTime">Optional endTime of bookings</param>
        /// <returns>List of <see cref="Booking"></see></returns>
        public async Task<IEnumerable<Booking>> ListBookingForSpecificUser(string userId, DateTime? startTime, DateTime? endTime)
        {
            if (startTime == null && endTime == null)
                return await bookingsRepo.GetAllUserBookingsFromNow(userId);
            else if (startTime == null)
                return await bookingsRepo.GetAllUserBookingsFromNow(userId, endTime.Value);
            else if (endTime == null)
                return await bookingsRepo.GetAllUserBookingsFrom(userId, startTime.Value);
            else
                return await bookingsRepo.GetAllUserBookingsFrom(userId, startTime.Value, endTime.Value);
        }

        /// <summary>
        /// Return list <see cref="Booking"></see> of specific  <see cref="Resource"></see>
        /// </summary>
        /// <param name="resourceId"><see cref="Booking.ResourceId"></see></param>
        /// <param name="asAdmin">If <see cref="true"></see> than return all bookings of specific <see cref="Resource"></see> else only booking from active in current time or in future</param>
        /// <returns></returns>
        public async Task<IEnumerable<Booking>> ListBookingOfResource(int resourceId, bool asAdmin)
        {
            if (asAdmin)
                return await bookingsRepo.GetBookingsOfResource(resourceId);
            else
                return await bookingsRepo.GetActiveBookingsOfResourceFromCurrentTime(resourceId);
        }

        /// <summary>
        /// Update exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see></param>
        /// <param name="startTime">Optional <see cref="Booking.StartTime"></see></param>
        /// <param name="endTime">Optional <see cref="Booking.EndTime"></see></param>
        /// <param name="editUser">User id who edit resource</param>
        /// <param name="note">Optional <see cref="Booking.Note"></see></param>
        /// <returns></returns>
        public async Task Update(int id, DateTime? startTime, DateTime? endTime, string editUser, string note)
        {
            await bookingsRepo.UpdateAsync(id, startTime, endTime, editUser, note);
        }

        /// <summary>
        /// Delete exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <returns></returns>
        public async Task Delete(int id)
        {
            await bookingsRepo.DeleteAsync(id);
        }

        #endregion CRUD operations

        #region Extensions

        #region Occupancy

        public async Task<double?> OccupancyByResource(int resourceId) => await bookingsRepo.OccupancyByResourceAsync(resourceId);

        #endregion Occupancy

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <param name="user">ID of <see cref="ApplicationUser"> which terminate booking</see></param>
        /// <returns></returns>
        public async Task Terminate(int id, string userId)
        {
            await bookingsRepo.Terminate(id, userId);
        }

        #endregion Extensions
    }
}