namespace BookingApp.Data.Models.Interfaces
{
    /// <summary>
    /// Defines an object that exposes 3-state activation status.
    /// </summary>
    public interface IActivable
    {
        /// <summary>
        /// 3-state activation status.
        /// </summary>
        bool? IsActive { get; set; }
    }
}
