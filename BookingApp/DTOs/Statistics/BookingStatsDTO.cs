using System;
using System.Collections.Generic;

namespace BookingApp.DTOs
{
    public class BookingStatsDTO
    {
        public BookingStatsDTO(List<BookingsPerResourceBaseDTO> list, DateTime[] xValues)
        {
            List = list;
            XValues = xValues;
        }

        public List<BookingsPerResourceBaseDTO> List { get; set; }
        public DateTime[] XValues { get; set; }
    }
}
