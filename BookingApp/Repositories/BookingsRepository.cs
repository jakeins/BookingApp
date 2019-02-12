using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
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
        readonly IQueryable<Booking> ActualBookings;


        public BookingsRepository(Data.ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Bookings = dbContext.Bookings;
            ActualBookings = dbContext.Bookings.Where(b => b.TerminationTime >= b.EndTime || b.TerminationTime == null);
        }

        #region CRUD operations

        /// <summary>
        /// Not implemented because Create must return id of newly created booking
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        async Task IRepositoryAsync<Booking, int>.CreateAsync(Booking model)
        {
            try
            {
                await dbContext.Database.ExecuteSqlCommandAsync(
                    $"EXEC [Booking.Create] {model.ResourceId}, '{model.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{model.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{model.CreatedUserId}', '{model.Note}'");
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on creating booking");
            }
        }

        /// <summary>
        /// Creating new <see cref="Booking"></see> using store procedure Booking.Create
        /// </summary>
        /// <param name="model">New <see cref="Booking"></see> data</param>
        /// <param name="user">User who create booking</param>
        /// <returns>Id of <see cref="Booking"/></returns>
        public async Task<int> CreateAsync(BookingCreateDTO model, string userId)
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
                    $"EXEC @retVal = [Booking.Create] {model.ResourceID}, '{model.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{model.EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff")}', '{userId}', '{model.Note}'",
                    param);
                return param.Value as int? ?? -1;
            }
            catch (SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on creating booking");
            }
            //Dummy throw. Unreachable code because ReThow not return
            throw new NullReferenceException();
        }

        /// <summary>
        /// Delete exist <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.BookingId"></see> of exist <see cref="Booking"></see></param>
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
        /// <param name="id"><see cref="Booking.BookingId"></see> of exis <see cref="Booking"></see></param>
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
                    $"EXEC [Booking.Edit] {model.BookingId}, {model.StartTime}, {model.EndTime}, {model.UpdatedUserId}, {model.Note}"
                    );
            }
            catch(SqlException ex)
            {
                Helpers.SqlExceptionTranslator.ReThrow(ex, "on edit booking");
            }
        }

        #endregion




        #region Extensions

        #region Occupancy
        /// <summary>
        /// Calculates single resource approx occupancy using the best way procedure.
        /// </summary>
        public async Task<double?> OccupancyByResourceAsync(int resourceId) => await OccupancyByResourceAsyncProc(resourceId);

        /// <summary>
        /// Calculates single resource approx occupancy using procedure.
        /// </summary>
        async Task<double?> OccupancyByResourceAsyncProc(int resourceId)
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
        async Task<double?> OccupancyByResourceAsyncEF(int resourceId)
        {
            if (!await dbContext.Resources.AnyAsync(e => e.ResourceId == resourceId))
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

            return occupiedMinutes / (firstEntry.PreOrderTimeLimit+firstEntry.MaxTime);
        }
        #endregion

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
        /// Get <see cref="Booking"></see> created by specified <see cref="ApplicationUser"></see> and active now or in future
        /// </summary>
        /// <param name="user"><see cref="Booking.CreatedUserId"></see></param>
        /// <returns>List of active books</returns>
        public async Task<IEnumerable<Booking>> GetActiveBookingsOfUserFromCurrentTime(ApplicationUser user) => await ActualBookings
                .Where(b => b.CreatedUserId == user.Id && (b.TerminationTime ?? b.EndTime) < DateTime.Now)
                .ToListAsync();

        /// <summary>
        /// Get all <see cref="Booking"></see> for specific <see cref="ApplicationUser"></see>
        /// </summary>
        /// <param name="user">Which books required</param>
        /// <returns>List of all books</returns>
        public async Task<IEnumerable<Booking>> GetBookingsOfUser(ApplicationUser user) => await ActualBookings
            .Where(b => b.CreatedUserId == user.Id)
            .ToListAsync();

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
        /// <param name="resource">Exist <see cref="Resource.ResourceId"></see></param>
        /// <returns>List of all <see cref="Booking"></see> of specific <see cref="Resource"></see></returns>
        public async Task<IEnumerable<Booking>> GetBookingsOfResource(int resourceId) => await Bookings
            .Where(b => b.ResourceId == resourceId)
            .ToListAsync();

        /// <summary>
        /// Terminate specific <see cref="Booking"></see>
        /// </summary>
        /// <param name="id"><see cref="Booking.BookingId"></see> of exist <see cref="Booking"></see></param>
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
        
        #endregion
    }
}
