using BookingApp.Data.Models;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public interface IBookingsRepository : IBasicRepositoryAsync<Booking, int>
    {
        /// <summary>
        /// Calculates single resource approx occupancy using the best way procedure.
        /// </summary>
        Task<double?> OccupancyByResourceAsync(int resourceId);

        /// <summary>
        /// Update exist <see cref="Booking"></see> data
        /// </summary>
        /// <param name="id">Id of exist <see cref="Booking"></see></param>
        /// <param name="startTime">Optional new <see cref="Booking.StartTime"></see>, if set than must date in future</param>
        /// <param name="endTime">Optional new <see cref="Booking.EndTime"></see>, if set than must date in future, stored as TerminationTime</param>
        /// <param name="editUserId">Editor user id, store as <see cref="Booking.UpdatedUserId"></see></param>
        /// <param name="note">Optional new <see cref="Booking.Note"></see></param>
        /// <returns></returns>
        Task UpdateAsync(int id, DateTime? startTime, DateTime? endTime, string editUser, string note);
        #region Get bookings for specific user
        /// <summary>
        /// Return all bookings in specific time range [StartTime,EndTime]
        /// </summary>
        /// <param name="userId">Id of <see cref="ApplicationUser"/></param>
        /// <param name="startTime">Start time of bookings</param>
        /// <param name="endTime">End time of bookings</param>
        /// <returns>List of <see cref="Booking"/></returns>
        Task<IEnumerable<Booking>> GetAllUserBookings(string userId, DateTime startTime, DateTime endTime);
        #endregion
        #region Get all bookings
        /// <summary>
        /// Return all bookings in specific time range [StartTime,EndTime]
        /// </summary>
        /// <param name="startTime">Start time of bookings</param>
        /// <param name="endTime">End time of bookings</param>
        /// <returns>List of <see cref="Booking"/></returns>
        Task<IEnumerable<Booking>> GetAllBookings(DateTime startTime, DateTime endTime);
        #endregion
        /// <summary>
        /// Get <see cref="Booking"></see> of specified <see cref="Resource"></see> and active now or in future
        /// </summary>
        /// <param name="resource">Booking <see cref="Resource"></see></param>
        /// <returns>List of active <see cref="Booking"></see></returns>
        Task<IEnumerable<Booking>> GetActiveBookingsOfResourceFromCurrentTime(int resourceId);

        /// <summary>
        /// Get all <see cref="Booking"></see> of specific <see cref="Resource"></see>
        /// </summary>
        /// <param name="resource">Exist <see cref="Resource.Id"></see></param>
        /// <returns>List of all <see cref="Booking"></see> of specific <see cref="Resource"></see></returns>
        Task<IEnumerable<Booking>> GetBookingsOfResource(int resourceId);

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <param name="user">ID of <see cref="ApplicationUser"> which terminate booking</see></param>
        /// <returns></returns>
        Task Terminate(int id, string userId);
    }

    public class BookingsRepository : IBookingsRepository
    {
        private Data.ApplicationDbContext dbContext;

        /// <summary>
        /// IQueryable shorthand for ApplicationDbContext.Bookings
        /// </summary>
        private readonly IQueryable<Booking> Bookings;

        /// <summary>
        /// IQueryable shorthand for only active ApplicationDbContext.Bookings
        /// </summary>
        private readonly IQueryable<Booking> ActualBookings;

        public BookingsRepository(Data.ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Bookings = dbContext.Bookings;
            ActualBookings = dbContext.Bookings.Where(b => b.TerminationTime >= b.EndTime || b.TerminationTime == null);
        }

        #region CRUD operations

        /// <summary>
        /// Create new booking and return id as <see cref="Booking.Id"/>
        /// </summary>
        /// <param name="model">On input booking data, on output Id of new booking</param>
        /// <returns></returns>
        public async Task CreateAsync(Booking model)
        {
            try
            {
                SqlParameter param = new SqlParameter
                {
                    ParameterName = "@retVal",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output,
                    Value = -1
                };

                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC @retVal = [Booking.Create] {model.Id}, '{model.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{model.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{model.CreatedUserId}', '{model.Note}'",
                    param);

                model.Id = param.Value as int? ?? -1;
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on creating booking");
            }
        }

        /// <summary>
        /// Delete exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            if (await GetAsync(id) is Booking item)
            {
                dbContext.Bookings.Remove(item);

                await SaveAsync();
            }
            else throw new Exceptions.EntryNotFoundException();
        }

        /// <summary>
        /// Return <see cref="Booking"></see> from Db
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exis <see cref="Booking"></see></param>
        /// <returns></returns>
        public async Task<Booking> GetAsync(int id) => await dbContext.Bookings.FindAsync(id);

        /// <summary>
        /// Return list of all <see cref="Booking"></see>
        /// </summary>
        /// <returns>List of all <see cref="Booking"></see></returns>
        public async Task<IEnumerable<Booking>> GetListAsync() => await Bookings.ToListAsync();

        /// <summary>
        /// Save all <see cref="Booking"></see> data
        /// </summary>
        /// <returns></returns>
        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Update <see cref="Booking"></see> using strore procedure Booking.Edit
        /// </summary>
        /// <param name="model"><see cref="Booking"></see> new data</param>
        /// <returns></returns>
        public async Task UpdateAsync(Booking model)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC [Booking.Edit] {model.Id}, {model.StartTime}, {model.EndTime}, {model.UpdatedUserId}, {model.Note}"
                    );
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on edit booking");
            }
        }

        #endregion CRUD operations

        #region Extensions

        #region Occupancy

        /// <summary>
        /// Calculates single resource approx occupancy using the best way procedure.
        /// </summary>
        public async Task<double?> OccupancyByResourceAsync(int resourceId) => await OccupancyByResourceAsyncProc(resourceId);

        /// <summary>
        /// Calculates single resource approx occupancy using procedure.
        /// </summary>
        private async Task<double?> OccupancyByResourceAsyncProc(int resourceId)
        {
            SqlParameter occupancyPercentsParam = new SqlParameter { ParameterName = "@occupancyPercents", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC @occupancyPercents = [Resource.OccupancyPercents] {resourceId}",
                    occupancyPercentsParam
                );
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "Resource Occupancy Calculation");
            }

            double procedureAnswer = (int)occupancyPercentsParam.Value;

            if (procedureAnswer < 0)
                return null;
            else
                return procedureAnswer / 100F;
        }

        /// <summary>
        /// Calculates single resource approx occupancy using EF Core.
        /// </summary>
        private async Task<double?> OccupancyByResourceAsyncEF(int resourceId)
        {
            if (!await dbContext.Resources.AnyAsync(e => e.Id == resourceId))
                throw new KeyNotFoundException("Specified resource doesn't exist.");

            var firstEntry = await dbContext.Bookings.Include(b => b.Resource).ThenInclude(b => b.Rule)
                .Where(booking => booking.ResourceId == resourceId)
                .Select(booking => new { booking.Resource.Rule.PreOrderTimeLimit, booking.Resource.Rule.MaxTime, booking.Resource.Rule.ServiceTime })
                .FirstOrDefaultAsync();

            if (firstEntry == null)//actual bookings absense means that resource is completely free
                return 0;

            if (firstEntry.PreOrderTimeLimit == null)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit not set.");
            else if (firstEntry.PreOrderTimeLimit < 0)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit cannot be negative.");
            else if (firstEntry.PreOrderTimeLimit == 0)
                return null;

            if (firstEntry.MaxTime == null)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit not set.");
            else if (firstEntry.MaxTime < 0)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit cannot be negative.");

            TimeSpan serviceTime = TimeSpan.FromMinutes(firstEntry.ServiceTime ?? 0);
            DateTime now = DateTime.Now;

            double occupiedMinutes = await dbContext.Bookings
                .Where(booking => booking.ResourceId == resourceId)
                .Select(b => (
                ((b.TerminationTime == null || b.TerminationTime <= b.EndTime) ? 1 : 0) * //absurdity check
                ((b.TerminationTime ?? b.EndTime).Subtract(b.StartTime > now ? b.StartTime : now) > new TimeSpan() ? // if calculated value is positive
                    (b.TerminationTime ?? b.EndTime).Subtract(b.StartTime > now ? b.StartTime : now) + serviceTime // then return it + service time
                    : new TimeSpan()) // else 0
                ).TotalMinutes
            )
            .SumAsync();

            return occupiedMinutes / (firstEntry.PreOrderTimeLimit + firstEntry.MaxTime);
        }

        #endregion Occupancy

        /// <summary>
        /// Update exist <see cref="Booking"></see> data
        /// </summary>
        /// <param name="id">Id of exist <see cref="Booking"></see></param>
        /// <param name="startTime">Optional new <see cref="Booking.StartTime"></see>, if set than must date in future</param>
        /// <param name="endTime">Optional new <see cref="Booking.EndTime"></see>, if set than must date in future, stored as TerminationTime</param>
        /// <param name="editUserId">Editor user id, store as <see cref="Booking.UpdatedUserId"></see></param>
        /// <param name="note">Optional new <see cref="Booking.Note"></see></param>
        /// <returns></returns>
        public async Task UpdateAsync(int id, DateTime? startTime, DateTime? endTime, string editUser, string note)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC [Booking.Edit] {id}, {startTime}, {endTime}, {editUser}, {note}"
                    );
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on edit booking");
            }
        }

        /// <summary>
        /// Get <see cref="Booking"></see> of specified <see cref="Resource"></see> and active now or in future
        /// </summary>
        /// <param name="resource">Booking <see cref="Resource"></see></param>
        /// <returns>List of active <see cref="Booking"></see></returns>
        public async Task<IEnumerable<Booking>> GetActiveBookingsOfResourceFromCurrentTime(int resourceId) => await ActualBookings
            .Where(b => b.ResourceId == resourceId && (b.TerminationTime ?? b.EndTime) < DateTime.Now)
            .ToListAsync();

        /// <summary>
        /// Get all <see cref="Booking"></see> of specific <see cref="Resource"></see>
        /// </summary>
        /// <param name="resource">Exist <see cref="Resource.Id"></see></param>
        /// <returns>List of all <see cref="Booking"></see> of specific <see cref="Resource"></see></returns>
        public async Task<IEnumerable<Booking>> GetBookingsOfResource(int resourceId) => await Bookings
            .Where(b => b.ResourceId == resourceId)
            .ToListAsync();

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.Id"></see> of exist <see cref="Booking"></see></param>
        /// <param name="user">ID of <see cref="ApplicationUser"> which terminate booking</see></param>
        /// <returns></returns>
        public async Task Terminate(int id, string userId)
        {
            if (await GetAsync(id) is Booking item)
            {
                try
                {
                    await dbContext.Database.ExecuteSqlCommandAsync(
                        $"EXEC [Booking.Terminate] {id}, {userId}"
                        );
                }
                catch (SqlException ex)
                {
                    Helpers.SqlExceptionTranslator.ReThrow(ex, "on terminate booking");
                }
            }
            else
                throw new Exceptions.EntryNotFoundException("Can not terminate not exist booking");
        }

        #region Get all bookings for specific user 

        /// <summary>
        /// Return all bookings in specific time range [StartTime,EndTime]
        /// </summary>
        /// <param name="userId">Id of <see cref="ApplicationUser"/></param>
        /// <param name="startTime">Start time of bookings</param>
        /// <param name="endTime">End time of bookings</param>
        /// <returns>List of <see cref="Booking"/></returns>
        public async Task<IEnumerable<Booking>> GetAllUserBookings(string userId, DateTime startTime, DateTime endTime) => await ActualBookings
                .Where(b => b.CreatedUserId == userId && (b.TerminationTime ?? b.EndTime) <= endTime && b.StartTime >= startTime)
                .ToListAsync();
        #endregion
        #region Get all bookings

        /// <summary>
        /// Return all bookings in specific time range [StartTime,EndTime]
        /// </summary>
        /// <param name="startTime">Start time of bookings</param>
        /// <param name="endTime">End time of bookings</param>
        /// <returns>List of <see cref="Booking"/></returns>
        public async Task<IEnumerable<Booking>> GetAllBookings(DateTime startTime, DateTime endTime) => await Bookings
            .Where(b => (b.TerminationTime ?? b.EndTime) < startTime && b.StartTime < endTime)
            .ToListAsync();
        #endregion

        #endregion Extensions
    }
}