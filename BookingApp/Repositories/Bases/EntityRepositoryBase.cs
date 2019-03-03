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
    /// Trackable Entity repository interface.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the entity primary id.</typeparam>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the primary id of the related user.</typeparam>
    public interface IEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        : IBasicRepositoryAsync<TEntity, TEntityKey>
        where TEntity : class, IEntity<TEntityKey, TUserModel, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserModel : class
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Get total count of all entities.
        /// </summary>
        Task<int> CountAsync();

        /// <summary>
        /// Checks whether provided entity exists.
        /// </summary>
        Task<bool> ExistsAsync(TEntity entity);

        /// <summary>
        /// Checks whether specified entity exists.
        /// </summary>
        Task<bool> ExistsAsync(TEntityKey id);

        /// <summary>
        /// Lists all entities which have the specified user as a creator OR updater.
        /// </summary>
        Task<IEnumerable<TEntity>> ListByAssociatedUser(TUserKey userId);

        /// <summary>
        /// Lists all entities which have the specified user as a creator.
        /// </summary>
        Task<IEnumerable<TEntity>> ListByCreator(TUserKey userId);

        /// <summary>
        /// Lists all entities which have the specified user as an updater.
        /// </summary>
        Task<IEnumerable<TEntity>> ListByUpdater(TUserKey userId);

        /// <summary>
        /// Lists identifiers of all entities.
        /// </summary>
        Task<IEnumerable<TEntityKey>> ListKeysAsync();

        /// <summary>
        /// Save changes do storage, wrapped in exception, verbose.
        /// </summary>
        Task SaveVerboseAsync(string saveReasonTitle);

        /// <summary>
        /// Updates only the properties, present in the provided <see cref="TSelectedProps"/>.
        /// /// <typeparam name="TSelectedProps">The class having all properties which should be updated.</typeparam>
        /// </summary>
        Task UpdateSelectiveAsync<TSelectedProps>(TEntity entity);
    }

    /// <summary>
    /// Base class for entity repository that uses EF.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the primary id.</typeparam>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the primary id of the related user.</typeparam>
    public abstract class EntityRepositoryBase<TEntity, TEntityKey, TUserModel, TUserKey>
        : IBasicRepositoryAsync<TEntity, TEntityKey>,
        IEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        where TEntity : class, IEntity<TEntityKey, TUserModel, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserModel : class
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

        #region Basic Repository Implementation

        public virtual async Task<IEnumerable<TEntity>> GetListAsync() => await Entities.ToListAsync();
        
        public virtual async Task<TEntity> GetAsync(TEntityKey id)
        {
            if (await Entities.SingleOrDefaultAsync(e => e.Id.Equals(id)) is TEntity entity)
                return entity;
            else
                throw NewNotFoundException;
        }
        
        public virtual async Task CreateAsync(TEntity entity)
        {
            Entities.Add(entity);
            await SaveVerboseAsync(EntityName + " Creation");
        }
        
        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (!await ExistsAsync(entity))
                throw NewNotFoundException;

            Entities.Update(entity);
            await SaveVerboseAsync(EntityName + " Update");
        }            
        
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
        
        public async Task UpdateSelectiveAsync<TSelectedProps>(TEntity entity)
        {
            if (!await ExistsAsync(entity))
                throw NewNotFoundException;

            //invalidating the exact properties for updating
            var updatedProps = typeof(TSelectedProps).GetProperties().Select(prop => prop.Name);
            foreach (var propName in updatedProps)
                dbContext.Entry(entity).Property(propName).IsModified = true;

            await SaveVerboseAsync(EntityName + " Update");
        }

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