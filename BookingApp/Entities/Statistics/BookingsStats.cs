using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Entities.Statistics
{
    public class BookingsStats
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

        public BookingsByResource BookingsByResource { get; set; }
    }
}
