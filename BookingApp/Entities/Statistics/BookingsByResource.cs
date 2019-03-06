using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Entities.Statistics
{
    public class BookingsByResource
    {
        public int ResourceId { get; set; }
        public string ResourceTitle { get; set; }
        /// <summary>
        /// Number of bookings for resource per interval in a timespan.
        /// </summary>
        public int[] BookingsNumber { get; set; }
    }
}
