using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookingApp.Data.Models;

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
        : IBasicRepositoryAsync<TEntity,TEntityKey>
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
}