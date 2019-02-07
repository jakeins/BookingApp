using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.DTOs;
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

        /// <summary>
        /// IQueryable shorthand for ApplicationDbContext.Resources
        /// </summary>
        readonly IQueryable<Resource> Resources;

        /// <summary>
        /// IQueryable shorthand for only active ApplicationDbContext.Resources
        /// </summary>
        readonly IQueryable<Resource> ActiveResources;

        public ResourcesRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            Resources = dbContext.Resources;
            ActiveResources = Resources.Where(r => r.IsActive == true);
        }

        #region CRUD operations

        public async Task<IEnumerable<Resource>> GetListAsync() => await Resources.ToListAsync();

        public async Task<Resource> GetAsync(int id) => await Resources.FirstOrDefaultAsync(p => p.ResourceId == id);

        public async Task CreateAsync(Resource item)
        {
            dbContext.Resources.Add(item);
            await SaveAsync();
        }

        public async Task UpdateAsync(Resource item)
        {
            if (await ResourceExistsAsync(item))
            {
                var propsToModify = typeof(ResourceDetailedDto).GetProperties()
                    .Where(prop => prop.Name != "ResourceId")
                    .Select(prop => prop.Name)
                    .Concat(new[] { "UpdatedUserId" });

                foreach (var propName in propsToModify)
                    dbContext.Entry(item).Property(propName).IsModified = true;

                try
                {
                    await SaveAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    string errorMessageTemplate = "Database operation expected to affect 1 row(s) but actually affected 0 row(s).";

                    if (ex.Message.Substring(0, errorMessageTemplate.Length) == errorMessageTemplate)
                        throw new OperationFailedException("Resource update failed due to concurrency issue.", ex);
                    else
                        throw;
                }
            }
            else
                throw new EntryNotFoundException();
        }

        public async Task DeleteAsync(int id)
        {
            if (await GetAsync(id) is Resource item)
            {
                dbContext.Resources.Remove(item);

                try
                {
                    await SaveAsync();
                }
                catch (DbUpdateException dbuException)
                {
                    if (dbuException.InnerException is SqlException sqlException && sqlException.Number == 547)
                        throw new OperationRestrictedException("Deletion cancelled because Resource has Bookings related upon it.", dbuException);
                    else
                        throw;
                }
            }
            else
                throw new EntryNotFoundException();
        }

        public async Task SaveAsync() => await dbContext.SaveChangesAsync();

        #endregion

        #region Public Extensions

        public async Task<bool> IsActiveAsync(int id)
        {
            return (await Resources.Where(r => r.ResourceId == id).Select(r => r.IsActive).SingleOrDefaultAsync()) == true;
        }

        public async Task<IEnumerable<Resource>> GetActiveListAsync() => await ActiveResources.ToListAsync();

        public async Task<IEnumerable<int>> ListIDsAsync() =>        await Resources.Select(r => r.ResourceId).ToListAsync();
        public async Task<IEnumerable<int>> ListActiveIDsAsync() =>  await ActiveResources.Select(r => r.ResourceId).ToListAsync();

        /// <summary>
        /// Calculates current approximate resource occupancy.
        /// </summary>
        public async Task<double?> CalculateSingleOccupancyAsync(int resourceId)
        {
            if (!await ResourceExistsAsync(resourceId))
                throw new KeyNotFoundException("Specified resource doesn't exist.");

            var firstEntry = await dbContext.Bookings.Include(b => b.Resource).ThenInclude(b => b.Rule)
                .Where(booking => booking.ResourceId == resourceId)
                .Select(booking => new { booking.Resource.Rule.PreOrderTimeLimit, booking.Resource.Rule.ServiceTime })
                .FirstOrDefaultAsync();

            if (firstEntry == null)//no booking => resource is completely free
                return 0;

            if (firstEntry.PreOrderTimeLimit == null)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit not set.");
            else if (firstEntry.PreOrderTimeLimit < 0)
                throw new FieldValueAbsurdException("Resource's rule PreOrderTimeLimit cannot be negative.");
            else if (firstEntry.PreOrderTimeLimit == 0)
                return null;

            TimeSpan serviceTime = TimeSpan.FromMinutes(firstEntry.ServiceTime ?? 0);
            var now = DateTime.Now;

            double occupiedMinutes = await dbContext.Bookings
            .Select(b => (
                (b.TerminationTime <= b.EndTime) &&
                ( (b.TerminationTime ?? b.EndTime) - (b.StartTime > now ? b.StartTime : now)  )
                ).TotalMinutes
            )
            .SumAsync();

            return occupiedMinutes / firstEntry.PreOrderTimeLimit;
        }
        #endregion

        #region Private Utilities
        /// <summary>
        /// Checks whether resource exists in database.
        /// </summary>
        async Task<bool> ResourceExistsAsync(int id) => await Resources.AnyAsync(e => e.ResourceId == id);

        /// <summary>
        /// Checks whether resource exists in database.
        /// </summary>
        async Task<bool> ResourceExistsAsync(Resource resource) => await ResourceExistsAsync(resource.ResourceId);
        #endregion
    }
}