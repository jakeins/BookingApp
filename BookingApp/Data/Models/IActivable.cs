namespace BookingApp.Data.Models
{
    /// <summary>
    /// Defines an entity that can be activated or deactivated.
    /// </summary>
    public interface IActivable
    {
        /// <summary>
        /// Provides deactivation functionality.
        /// </summary>
        bool? IsActive { get; set; }
    }
}
