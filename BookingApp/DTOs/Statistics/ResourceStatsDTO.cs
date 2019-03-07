using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class ResourceStatsDTO
    {
        public int ResourceId { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// Average booking period for the resouce.
        /// </summary>
        public TimeSpan AverageTime { get; set; }
        /// <summary>
        /// Minimal booking period for the resouce.
        /// </summary>
        public TimeSpan MinTime { get; set; }
        /// <summary>
        /// Maximum booking period for the resouce.
        /// </summary>
        public TimeSpan MaxTime { get; set; }
        /// <summary>
        /// Most frequent booking period for the resource.
        /// </summary>
        public TimeSpan ModeTime { get; set; }
        /// <summary>
        /// Cancelled bookings to all bookings ratio.
        /// </summary>
        public double CancellationRate { get; set; }
        /// <summary>
        /// Average booking period to max bookings period (from resource's rule) ratio.
        /// </summary>
        public double AverageUsageRate { get; set; }
    }
}
