using System;

namespace BookingApp.Data.Models.Interfaces
{
    /// <summary>
    /// Entity that can be trackable thanks to having primary id and created/updated user/time.
    /// </summary>
    /// <typeparam name="TUserModel">Type of the related user object.</typeparam>
    /// <typeparam name="TUserKey">Type of the related user primary key (id).</typeparam>
    public interface ITrackable<TUserModel, TUserKey>
        where TUserModel : class
        where TUserKey : IEquatable<TUserKey>
    {
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
        TUserModel Creator { get; set; }

        /// <summary>
        /// User who updated current entry.
        /// </summary>
        TUserModel Updater { get; set; }
    }
}