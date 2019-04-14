using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs.Statistics
{
    public class ResourceStatsBriefDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// All time count of bookings for the resource.
        /// </summary>
        public int BookingsCount { get; set; }
    }
}
