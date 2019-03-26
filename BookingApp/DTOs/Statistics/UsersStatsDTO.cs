namespace BookingApp.DTOs.Statistics
{
    public class UsersStatsDTO
    {
        /// <summary>
        /// Count of approved registered users.
        /// </summary>
        public int RegisteredUsers { get; set; }
        /// <summary>
        /// Count of users that made bookings in last 30 days
        /// </summary>
        public int ActiveUsers { get; set; }
        /// <summary>
        /// Count of bloked users
        /// </summary>
        public int BlockedUsers { get; set; }
    }
}
