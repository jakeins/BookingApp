using System;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Defines basic set of entity properties. Requires the Entity Id Type as a type parameter.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of the primary key (id).</typeparam>
    /// <typeparam name="TUserKey">Type of the primary key (id) of the related user.</typeparam>
    public interface IEntity<TEntityKey,TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Id of the enitity.
        /// </summary>
        TEntityKey Id { get; set; }

        /// <summary>
        /// Time of the current entry creation. Gets set automatically by the persistent storage.
        /// </summary>
        DateTime CreatedTime { get; set; }

        /// <summary>
        /// Time of the current entry creation. Gets set automatically by the persistent storage.
        /// </summary>
        DateTime UpdatedTime { get; set; }

        /// <summary>
        /// Identifier of the user who created current entry.
        /// </summary>
        TUserKey CreatedUserId { get; set; }

        /// <summary>
        /// Identifier of the user who updated current entry.
        /// </summary>
        TUserKey UpdatedUserId { get; set; }

        /// <summary>
        /// User who created current entry.
        /// </summary>
        ApplicationUser Creator { get; set; }

        /// <summary>
        /// User who updated current entry.
        /// </summary>
        ApplicationUser Updater { get; set; }
    }
}
