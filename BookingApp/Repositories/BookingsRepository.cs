using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Repositories
{
    public class BookingsRepository : IRepositoryAsync<Booking, int>
    {
        Data.ApplicationDbContext dbContext;

        /// <summary>
        /// IQueryable shorthand for ApplicationDbContext.Bookings
        /// </summary>
        readonly IQueryable<Booking> Bookings;

        /// <summary>
        /// IQueryable shorthand for only active ApplicationDbContext.Bookings
        /// </summary>
        readonly IQueryable<Booking> ActiveBookings;


        public BookingsRepository(Data.ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Bookings = dbContext.Bookings;
            ActiveBookings = dbContext.Bookings.Where(b => b.TerminationTime != b.EndTime);
        }

        #region CRUD operations

        /// <summary>
        /// Creating new booking using store procedure Booking.Create
        /// </summary>
        /// <param name="model">new booking data</param>
        /// <returns></returns>
        public async Task CreateAsync(Booking model)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC BookingCreate",
                    new SqlParameter("@ResourceID", model.ResourceId),
                    new SqlParameter("@StartTime", model.StartTime),
                    new SqlParameter("@EntTime", model.EndTime),
                    new SqlParameter("@UserID", model.CreatedUserId),
                    new SqlParameter("@Note", model.Note)
                    );

            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on creating booking");
            }
        }

        public async Task DeleteAsync(int id)
        {
            if(await GetAsync(id) is Booking item){
                dbContext.Bookings.Remove(item);

                await SaveAsync();
            }
        }

        public async Task<Booking> GetAsync(int id) => await dbContext.Bookings.FindAsync(id);

        public async Task<IEnumerable<Booking>> GetListAsync() => await Bookings.ToListAsync();

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update using strore procedure Booking.Edit
        /// </summary>
        /// <param name="model">booking new data</param>
        /// <returns></returns>
        public async Task UpdateAsync(Booking model)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC Booking.Edit",
                    new SqlParameter("@BookingID", model.BookingId),
                    new SqlParameter("@StartTime", model.StartTime),
                    new SqlParameter("@EndTime", model.EndTime),
                    new SqlParameter("@EditUseID", model.UpdatedUserId),
                    new SqlParameter("@Note", model.Note)
                    );
            }
            catch(SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on edit booking");
            }
        }

        #endregion

        #region Public Extensions

        /// <summary>
        /// Creating new booking by setting fields
        /// </summary>
        /// <param name="resource">Booking resource, must be enabled</param>
        /// <param name="startTime">StartTime of booking, must be in future and lower EndTime</param>
        /// <param name="endTime">EndTime of booking, must be in future</param>
        /// <param name="createdBy">User who book</param>
        /// <param name="note">Additional note</param>
        /// <returns></returns>
        public async Task CreateAsync(Resource resource, DateTime startTime, DateTime endTime, ApplicationUser createdBy, string note)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC BookingCreate",
                    new SqlParameter("@ResourceID", resource.ResourceId),
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@EntTime", endTime),
                    new SqlParameter("@UserID", createdBy.Id),
                    new SqlParameter("@Note", note)
                    );

            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on creating booking");
            }
        }

        /// <summary>
        /// Update booking data
        /// </summary>
        /// <param name="id">Id of exist booking</param>
        /// <param name="startTime">Optional new StartTime, if set than must date in future</param>
        /// <param name="endTime">Optional new EndTime, if set than must date in future, stored as TerminationTime</param>
        /// <param name="editUserId">Id of user, store as UpdateUserID</param>
        /// <param name="note">Optional booking description</param>
        /// <returns></returns>
        public async Task UpdateAsync(int id, DateTime startTime, DateTime endTime, ApplicationUser editUser, string note)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC Booking.Edit",
                    new SqlParameter("@BookingID", id),
                    new SqlParameter("@StartTime", startTime),
                    new SqlParameter("@EndTime", endTime),
                    new SqlParameter("@EditUseID", editUser.Id),
                    new SqlParameter("@Note", note)
                    );
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on edit booking");
            }
        }

        /// <summary>
        /// Get booking created by specified user and active now or in future
        /// </summary>
        /// <param name="user">Which books required</param>
        /// <returns>List of active books</returns>
        public async Task<IEnumerable<Booking>> GetActiveBookingsOfUserFromCurrentTime(ApplicationUser user) => await ActiveBookings
                .Where(b => b.CreatedUserId == user.Id && (b.TerminationTime ?? b.EndTime) < DateTime.Now)
                .ToListAsync();

        /// <summary>
        /// Get all books for specific user
        /// </summary>
        /// <param name="user">Which books required</param>
        /// <returns>List of all books</returns>
        public async Task<IEnumerable<Booking>> GetBookingsOfUser(ApplicationUser user) => await ActiveBookings
            .Where(b => b.CreatedUserId == user.Id)
            .ToListAsync();

        /// <summary>
        /// Get booking of specified resource and active now or in future
        /// </summary>
        /// <param name="resource">Booking resource</param>
        /// <returns>List of active bookings</returns>
        public async Task<IEnumerable<Booking>> GetActiveBookingsOfResourceFromCurrentTime(int resourceId) => await ActiveBookings
            .Where(b => b.ResourceId == resourceId && (b.TerminationTime ?? b.EndTime) < DateTime.Now)
            .ToListAsync();

        /// <summary>
        /// Get all booking of specific resource
        /// </summary>
        /// <param name="resource"></param>
        /// <returns>List of all bookings</returns>
        public async Task<IEnumerable<Booking>> GetBookingsOfResource(int resourceId) => await Bookings
            .Where(b => b.ResourceId == resourceId)
            .ToListAsync();

        #endregion
    }
}
