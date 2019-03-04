using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.DTOs
{
    public class BookingsPerResourceAllDTO
    {
        public string ResourceTitle { get; set; }
        public int[] BookingsPerInterval { get; set; }
        public int BookingsSum { get; set; }
    }
}
