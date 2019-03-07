using System;

namespace BookingApp.Data.Models.Interfaces
{
    /// <summary>
    /// Object that has id.
    /// </summary>
    /// <typeparam name="TEntityKey">Type of the entity primary key (id).</typeparam>
    public interface IIdentifiable<TEntityKey>
        where TEntityKey : IEquatable<TEntityKey>
    {
        /// <summary>
        /// Id of the object.
        /// </summary>
        TEntityKey Id { get; set; }
    }
}