using BookingApp.Data;
using BookingApp.Data.Models.Interfaces;
using BookingApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Bases
{
    /// <summary>
    /// Base class for entity repository that uses EF.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the primary id.</typeparam>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the primary id of the related user.</typeparam>
    public abstract class TrackEntityRepositoryBase<TEntity, TEntityKey, TUserModel, TUserKey>
        : BasicRepositoryBase<TEntity, TEntityKey>,
        ITrackEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        where TEntity : class, IIdentifiable<TEntityKey>, ITrackable<TUserModel, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserModel : class
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TrackEntityRepositoryBase(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task UpdateSelectiveAsync<TSelectedProps>(TEntity entity)
        {
            if (!await ExistsAsync(entity))
                throw NewNotFoundException;

            //invalidating the exact properties for updating
            var updatedProps = typeof(TSelectedProps).GetProperties().Select(prop => prop.Name);
            foreach (var propName in updatedProps)
                dbContext.Entry(entity).Property(propName).IsModified = true;

            await SaveAsync();
        }

        public async Task<IEnumerable<TEntityKey>> ListKeysAsync() => await Entities.Select(e => e.Id).ToListAsync();
        
        public async Task<bool> ExistsAsync(TEntityKey id) => await Entities.AnyAsync(e => e.Id.Equals(id));

        public async Task<bool> ExistsAsync(TEntity entity) => await ExistsAsync(entity.Id);

        public async Task<int> CountAsync() => await Entities.CountAsync();

        public async Task<IEnumerable<TEntity>> ListByAssociatedUser(TUserKey userId)
        {
            return await Entities
                .Where(e => userId.Equals(e.CreatedUserId) || userId.Equals(e.UpdatedUserId))
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TEntity>> ListByCreator(TUserKey userId)
        {
            return await Entities
                .Where(e => userId.Equals(e.CreatedUserId))
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TEntity>> ListByUpdater(TUserKey userId)
        {
            return await Entities
                .Where(e => userId.Equals(e.UpdatedUserId))
                .ToListAsync();
        }
    }
}