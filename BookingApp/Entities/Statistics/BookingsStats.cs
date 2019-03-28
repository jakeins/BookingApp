using System;
using System.Collections.Generic;
using BookingApp.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Entities.Statistics
{
    public class BookingsStats
    {
        public BookingsStats(BookingStatsTypes type, DateTime fromDate, DateTime toDate, string interval, DateTime[] intervalsValues, int[] bookingsAll, Dictionary<int, int[]> bookingsByResources)
        {
            Type = type.ToString();
            FromDate = fromDate;
            ToDate = toDate;
            Interval = interval;
            IntervalsValues = intervalsValues;
            BookingsAll = bookingsAll;
            BookingsByResources = bookingsByResources;
        }

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
        public Dictionary<int,int[]> BookingsByResources { get; set; }


    }
}
