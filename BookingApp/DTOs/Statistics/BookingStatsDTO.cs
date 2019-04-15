using System;
using System.Collections.Generic;

namespace BookingApp.DTOs.Statistics
{
    public class BookingStatsDTO
    {
        /// <summary>
        /// Type of actions with bookings (e.g. creations, cancellations, completions, terminations).
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Requested timespan start.
        /// </summary>
        public DateTime FromDate { get; set; }
        /// <summary>
        /// Requested timespan end.
        /// </summary>
        public DateTime ToDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Interval { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime[] IntervalsValues { get; set; }
        /// <summary>
        /// Sum of bookings (all resources) per interval in specified time span. 
        /// </summary>
        public int[] BookingsAll { get; set; }
        /// <summary>
        /// Collections of bookings filtered by resourceID per interval in specified time span.
        /// Key = resourceId, Value = number of bookings for resource per interval.
        /// </summary>
        public Dictionary<int, int[]> BookingsByResources { get; set; }
    }
}
