using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Entities.Statistics;

namespace BookingApp.Services.Interfaces
{
    public interface IStatisticsService
    {
        Task<BookingsStats> GetBookingsCreations(DateTime start, DateTime end, string interval, int[] resourcesIDs);

        Task<BookingsStats> GetBookingsCancellations(DateTime start, DateTime end, string interval, int[] resourcesIDs);

        Task<BookingsStats> GetBookingsTerminations(DateTime start, DateTime end, string interval, int[] resourcesIDs);

        Task<BookingsStats> GetBookingsCompletions(DateTime start, DateTime end, string interval, int[] resourcesIDs);

        Task<IEnumerable<ResourceStats>> GetResourceStats();

        Task<ResourceStats> GetResourceStats(int resourceID);

        Task<IEnumerable<ResourceStats>> GetResourcesRating();

        Task<UsersStats> GetUsersStats();
    }
}
