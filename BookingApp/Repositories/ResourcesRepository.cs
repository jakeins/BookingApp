using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    public class ResourcesRepository : IRepositoryAsync<Resource, int>
    {
        ApplicationDbContext dbContext;

        public ResourcesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region CRUD operations

        public async Task<IEnumerable<Resource>> GetListAsync() => await dbContext.Resources.ToListAsync();

        public async Task<Resource> GetAsync(int id) => await dbContext.Resources.FirstOrDefaultAsync(p => p.ResourceId == id);

        public async Task CreateAsync(Resource item)
        {
            dbContext.Resources.Add(item);
            await SaveAsync();
        }

        public async Task UpdateAsync(Resource item)
        {
            dbContext.Resources.Update(item);

            try
            {
                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                string errorMessageTemplate = "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

                if (ex.Message.Substring(0, errorMessageTemplate.Length) == errorMessageTemplate)
                    throw new UpdateFailedException("Resource update failed because of missing entry or some concurrency issue.", ex);
                else
                    throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (await dbContext.Resources.FirstOrDefaultAsync(p => p.ResourceId == id) is Resource item)
            {
                dbContext.Resources.Remove(item);

                try
                {
                    await SaveAsync();
                }
                catch (DbUpdateException dbuException)
                {
                    if (dbuException.InnerException is SqlException sqlException && sqlException.Number == 547)
                        throw new DeleteResctrictedException("Deletion cancelled because Resource has Bookings related upon it.", dbuException);
                    else
                        throw;
                }
            }
        }

        public async Task SaveAsync() => await dbContext.SaveChangesAsync();

        #endregion

        #region Public Extensions

        public async Task<bool> IsResourceActive(int id)
        {
            return (await Alls().Where(r => r.ResourceId == id).Select(r => r.IsActive).SingleOrDefaultAsync()) == true;
        }

        /// <summary>
        /// Gets resources list filtered by their active status.
        /// </summary>
        public async Task<IEnumerable<Resource>> GetList(bool showIncatives)
        {
            if (showIncatives)
                return await GetListAsync();
            else
                return await Actives().ToListAsync();
        }

        public async Task<IEnumerable<int>> GetIDsList(bool includeInactives)
        {
            return await ( includeInactives ? Alls() : Actives() ).Select(r => r.ResourceId).ToListAsync();
        }

        /// <summary>
        /// Calculates current approximate resource occupancy.
        /// </summary>
        public async Task<double?> GetResourceOccupancy(int resourceId)
        {
            if (!await ResourceExists(resourceId))
                throw new KeyNotFoundException("Specified resource doesn't exist.");

            var firstEntry = await dbContext.Bookings.Include(b => b.Resource).ThenInclude(b => b.Rule)
                .Where(booking => booking.ResourceId == resourceId)
                .Select(booking => new { booking.Resource.Rule.PreOrderTimeLimit })
                .FirstOrDefaultAsync();

            if (firstEntry == null)//no booking => resource is completely free
                return 0;

            if (firstEntry.PreOrderTimeLimit == null)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit not set.");
            else if (firstEntry.PreOrderTimeLimit < 0)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit cannot be negative.");
            else if (firstEntry.PreOrderTimeLimit == 0)
                return null;

            var now = DateTime.Now;

            double occupiedMinutes = await dbContext.Bookings.Include(b => b.Resource).ThenInclude(b => b.Rule)
            .Where(booking =>
                booking.ResourceId == resourceId &&
                booking.EndTime + TimeSpan.FromMinutes(booking.Resource.Rule.ServiceTime ?? 0) > now
            )
            .Select(booking =>
                (booking.EndTime + TimeSpan.FromMinutes(booking.Resource.Rule.ServiceTime ?? 0) - new[] { booking.StartTime, now }.Max())
                .TotalMinutes
            )
            .SumAsync();

            return occupiedMinutes / firstEntry.PreOrderTimeLimit;
        }
        #endregion

        #region Private Utilities

        /// <summary>
        /// Checks whether resource exists in database.
        /// </summary>
        async Task<bool> ResourceExists(int id) => await dbContext.Resources.AnyAsync(e => e.ResourceId == id);

        /// <summary>
        /// IQueryable preselect of all resources.
        /// </summary>
        IQueryable<Resource> Alls() => dbContext.Resources;

        /// <summary>
        /// IQueryable preselect of all active resources.
        /// </summary>
        IQueryable<Resource> Actives() => dbContext.Resources.Where(r => r.IsActive == true);
        #endregion
    }
}
