using System;

namespace BookingApp.Entities.Statistics
{
    public class ResourceStats
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int BookingsCount { get; set; }
        public TimeSpan AverageTime { get; set; }
        public TimeSpan MinTime { get; set; }
        public TimeSpan MaxTime { get; set; }
        public TimeSpan ModeTime { get; set; }
        public double CancellationRate { get; set; }
        public double AverageUsageRate { get; set; }
    }
}
