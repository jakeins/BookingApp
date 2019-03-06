using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class ResourceStatsDTO
    {
        public string Title { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }
        public TimeSpan ModeTime { get; set; }
        public double CancellationRate { get; set; }
        public double AverageUsageRate { get; set; }
    }
}
