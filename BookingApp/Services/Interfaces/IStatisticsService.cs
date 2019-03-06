﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Entities.Statistics;

namespace BookingApp.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<BookingsStats> GetBookingsCreations(DateTime start, DateTime end, string interval);

        Task<BookingsStats> GetBookingsCancellations(DateTime start, DateTime end, string interval);

        Task<BookingsStats> GetBookingsTerminations(DateTime start, DateTime end, string interval);

        Task<BookingsStats> GetBookingsCompletions(DateTime start, DateTime end, string interval);
    }
}