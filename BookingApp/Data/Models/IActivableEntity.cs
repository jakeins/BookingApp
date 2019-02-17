using System;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Defines basic set of properties for the entity that has IsActive flag. Requires the Entity Id Type as a type parameter.
    /// </summary>
    public interface IActivableEntity<TEntityKey, TUserKey> : IEntity<TEntityKey, TUserKey>
        where TEntityKey : IEquatable<TEntityKey>
        where TUserKey : IEquatable<TUserKey>
    {
        /// <summary>
        /// Provides deactivation functionality.
        /// </summary>
        bool? IsActive { get; set; }
    }
}
