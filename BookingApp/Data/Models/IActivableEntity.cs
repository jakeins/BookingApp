namespace BookingApp.Data.Models
{
    /// <summary>
    /// Defines basic set of properties for the entity that has IsActive flag. Requires the Entity Id Type as a type parameter.
    /// </summary>
    public interface IActivableEntity<EntityIdType> : IEntity<EntityIdType>
    {
        /// <summary>
        /// Provides deactivation functionality.
        /// </summary>
        bool? IsActive { get; set; }
    }
}
