using System;

namespace BookingApp.Data.Models
{
    /// <summary>
    /// Defines basic set of entity properties. Requires the Entity Id Type as a type parameter.
    /// </summary>
    public interface IEntity<EntityIdType>
    {
        /// <summary>
        /// Id of the enitity.
        /// </summary>
        EntityIdType Id { get; set; }

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
        string CreatedUserId { get; set; }

        /// <summary>
        /// Identifier of the user who updated current entry.
        /// </summary>
        string UpdatedUserId { get; set; }

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
