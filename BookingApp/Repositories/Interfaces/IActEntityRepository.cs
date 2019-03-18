using BookingApp.Data.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Repositories.Interfaces
{
    /// <summary>
    /// Activable Trackable Entity repository interface.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity object.</typeparam>
    /// <typeparam name="TEntityKey">Type of the entity primary id.</typeparam>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the primary id of the related user.</typeparam>
    public interface IActEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        : ITrackEntityRepository<TEntity, TEntityKey, TUserModel, TUserKey>
        where TEntity : class, ITrackable<TUserModel, TUserKey>, IActivable
        where TEntityKey : IEquatable<TEntityKey>
        where TUserModel : class
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Get count of all active entities.
        /// </summary>
        Task<int> CountActiveAsync();

        /// <summary>
        /// Checks whether sepcified entity is active.
        /// </summary>
        Task<bool> IsActiveAsync(TEntityKey id);

        /// <summary>
        /// Lists active entities.
        /// </summary>
        Task<IEnumerable<TEntity>> ListActiveAsync();

        /// <summary>
        /// Lists identifiers of all active entities.
        /// </summary>
        Task<IEnumerable<TEntityKey>> ListActiveKeysAsync();
    }
}