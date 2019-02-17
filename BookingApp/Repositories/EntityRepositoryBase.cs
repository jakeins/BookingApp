using BookingApp.Data;
using BookingApp.Data.Models;
using BookingApp.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Repositories
{
    /// <summary>
    /// Base class for repository that uses EF.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity.</typeparam>
    /// /// <typeparam name="TEntityKey">Type of the primary key (id).</typeparam>
    /// <typeparam name="TUserKey">Type of the primary key (id) of the related user.</typeparam>
    public abstract class EntityRepositoryBase<TEntity, TEntityKey, TUserKey> 
        where TEntity : class, IEntity<TEntityKey, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserKey : IEquatable<TUserKey>
    {
        protected ApplicationDbContext dbContext;

        /// <summary>
        /// Name of the current entity, default is the model class name.
        /// </summary>
        protected string EntityName = typeof(TEntity).Name;

        /// <summary>
        /// Shorthand DbSet for Entities.
        /// </summary>
        protected DbSet<TEntity> Entities => dbContext.Set<TEntity>();

        /// <summary>
        /// Not found exception factory
        /// </summary>
        protected CurrentEntryNotFoundException NewNotFoundException => new CurrentEntryNotFoundException($"Specified {EntityName} not found");

        /// <summary>
        /// Constructor.
        /// </summary>
        public EntityRepositoryBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        #region Standard repository operations
        /// <summary>
        /// List all entries.
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> GetListAsync() => await Entities.ToListAsync();

        /// <summary>
        /// Gets specified entry.
        /// </summary>
        public virtual async Task<TEntity> GetAsync(TEntityKey id)
        {
            if (await Entities.SingleOrDefaultAsync(e => e.Id.Equals(id)) is TEntity entity)
                return entity;
            else
                throw NewNotFoundException;
        }

        /// <summary>
        /// Creates specified enitity in the storage.
        /// </summary>
        public virtual async Task CreateAsync(TEntity entity)
        {
            Entities.Add(entity);
            await SaveVerboseAsync(EntityName + " Creation");
        }

        /// <summary>
        /// Rewrites storage entity entirely with the provided model.
        /// </summary>
        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (!await ExistsAsync(entity))
                throw NewNotFoundException;

            Entities.Update(entity);
            await SaveVerboseAsync(EntityName + " Update");
        }            

        /// <summary>
        /// Deletes specified entity.
        /// </summary>
        public virtual async Task DeleteAsync(TEntityKey id)
        {
            if (await GetAsync(id) is TEntity entity)
            {
                Entities.Remove(entity);

                await SaveVerboseAsync(EntityName + " Deletion");
            }
            else
                throw NewNotFoundException;
        }

        /// <summary>
        /// Save changes do storage, wrapped in exception.
        /// </summary>
        public virtual async Task SaveAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbuException)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException);
            }
        }
        #endregion

        #region Extensions
        /// <summary>
        /// Updates only the properties, present in the provided <see cref="UpdatePropertiesAggregationType"/>.
        /// </summary>
        /// <typeparam name="UpdatePropertiesAggregationType">The class having all properties which should be updated.</typeparam>
        public async Task UpdateSelectiveAsync<UpdatePropertiesAggregationType>(TEntity entity)
        {
            if (!await ExistsAsync(entity))
                throw NewNotFoundException;

            //invalidating the exact properties for updating
            var updatedProps = typeof(UpdatePropertiesAggregationType).GetProperties().Select(prop => prop.Name);
            foreach (var propName in updatedProps)
                dbContext.Entry(entity).Property(propName).IsModified = true;

            await SaveVerboseAsync(EntityName + " Update");
        }

        /// <summary>
        /// Save changes do storage, wrapped in exception, verbose.
        /// </summary>
        public async Task SaveVerboseAsync(string saveReasonTitle)
        {
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbuException)
            {
                Helpers.DbUpdateExceptionTranslator.ReThrow(dbuException, saveReasonTitle);
            }
        }

        /// <summary>
        /// Lists identifiers of all entities.
        /// </summary>
        public async Task<IEnumerable<TEntityKey>> ListIDsAsync() => await Entities.Select(e => e.Id).ToListAsync();

        /// <summary>
        /// Checks whether specified entity exists.
        /// </summary>
        public async Task<bool> ExistsAsync(TEntityKey id) => await Entities.AnyAsync(e => e.Id.Equals(id));

        /// <summary>
        /// Checks whether specified entity exists.
        /// </summary>
        public async Task<bool> ExistsAsync(TEntity entity) => await ExistsAsync(entity.Id);

        /// <summary>
        /// Lists all entities which have the specified user as a creator OR updater.
        /// </summary>
        public async Task<IEnumerable<TEntity>> ListByAssociatedUser(TUserKey userId)
        {
            return await Entities.
                Where(e => userId.Equals(e.CreatedUserId) || userId.Equals(e.UpdatedUserId))
                .ToListAsync();
        }

        /// <summary>
        /// Lists all entities which have the specified user as a creator.
        /// </summary>
        public async Task<IEnumerable<TEntity>> ListByCreator(TUserKey userId) => await Entities.Where(e => userId.Equals(e.CreatedUserId)).ToListAsync();

        /// <summary>
        /// Lists all entities which have the specified user as an updater.
        /// </summary>
        public async Task<IEnumerable<TEntity>> ListByUpdater(TUserKey userId) => await Entities.Where(e => userId.Equals(e.UpdatedUserId)).ToListAsync();
        #endregion
    }
}