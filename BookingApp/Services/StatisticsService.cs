using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.Data.Models;
using BookingApp.Entities.Statistics;
using BookingApp.Services.Interfaces;
using BookingApp.Repositories;

namespace BookingApp.Services
{
    public class StatisticsService : IStatisticsService
    {
        private IBookingsRepository bookingsRepository;
        private IResourcesRepository resourcesRepository;

        public StatisticsService(IBookingsRepository bookingRepo, IResourcesRepository resourcesRepo)
        {
            bookingsRepository = bookingRepo;
            resourcesRepository = resourcesRepo;
        }

        public async Task<BookingsStats> GetBookingsCancellations(DateTime start, DateTime end, string interval, int[] resourcesIDs)
        {
            IEnumerable<Booking> bookings = await bookingsRepository.GetListAsync();
            IEnumerable<Booking> cancelledbookings = 
                bookings.Where(b => b.TerminationTime != null && b.TerminationTime >= start && b.TerminationTime <= end).Where(b=>b.TerminationTime<b.StartTime);
            return GetBookingStats("cancellation", cancelledbookings, start, end, interval, resourcesIDs);
        }

        public async Task<BookingsStats> GetBookingsCompletions(DateTime start, DateTime end, string interval, int[] resourcesIDs)
        {
            IEnumerable<Booking> bookings = await bookingsRepository.GetListAsync();
            IEnumerable<Booking> completedbookings =
                bookings.Where(b => b.TerminationTime == null && b.EndTime >= start && b.EndTime <= end);
            return GetBookingStats("completion", completedbookings, start, end, interval, resourcesIDs);
        }

        public async Task<BookingsStats> GetBookingsCreations(DateTime start, DateTime end, string interval, int[] resourcesIDs)
        {
            IEnumerable<Booking> bookings = await bookingsRepository.GetListAsync();
            IEnumerable<Booking> createdbookings =
                bookings.Where(b => b.CreatedTime >= start && b.CreatedTime <= end);
            return GetBookingStats("creation", createdbookings, start, end, interval, resourcesIDs);
        }

        public async Task<BookingsStats> GetBookingsTerminations(DateTime start, DateTime end, string interval, int[] resourcesIDs)
        {
            IEnumerable<Booking> bookings = await bookingsRepository.GetListAsync();
            IEnumerable<Booking> terminatedbookings =
                bookings.Where(b => b.TerminationTime != null && b.TerminationTime >= start && b.TerminationTime <= end).Where(b => b.TerminationTime > b.StartTime);
            return GetBookingStats("termination", terminatedbookings, start, end, interval, resourcesIDs);

        }

        public Task<ResourceStats> GetResourceStats()
        {
            throw new NotImplementedException();
        }

        public Task<ResourceStats> GetResourceStats(int resourceID)
        {
            throw new NotImplementedException();
        }

        #region Helpers

        private BookingsStats GetBookingStats(string type, IEnumerable<Booking> bookings,DateTime start,DateTime end,string interval, int[] ids)
        {
            int intervalsNumber = GetIntervalsNumber(start, end, interval) + 1;// adding 1 to include end
            DateTime[] intervalsValues = GetIntervalValues(start, interval, intervalsNumber);
            int[] all = new int[intervalsNumber];
            Dictionary<int, int[]> bookingsOfResource = new Dictionary<int, int[]>();

            foreach(var booking in bookings)
            {
                int intervalIndex = GetIntervalsNumber(start, GetBookingDateByType(booking, type),interval);
                all[intervalIndex]++;
                if (ids.Length==0 || ids.Contains(booking.ResourceId))
                {
                    if (bookingsOfResource.ContainsKey(booking.ResourceId))
                    {
                        bookingsOfResource[booking.ResourceId][intervalIndex]++;
                    }
                    else
                    {
                        bookingsOfResource.Add(booking.ResourceId, new int[intervalsNumber]);
                        bookingsOfResource[booking.ResourceId][intervalIndex]++;
                    }
                }
            }

            return new BookingsStats(type, start, end, interval, intervalsValues, all, bookingsOfResource);
        }
        
        private DateTime GetBookingDateByType(Booking booking, string type)
        {
            switch(type)
            {
                case "creation":
                    return booking.CreatedTime;
                case "cancellation":                    
                case "termination":
                    return booking.TerminationTime.Value;
                case "completion":
                    return booking.EndTime;
                default:
                    throw new ApplicationException("Only creation, cancellation, termination, cmpletion type is allowed");
            }
        }

        private int GetIntervalsNumber(DateTime start, DateTime end, string interval)
        {
            int number = 0;

            switch (interval)
            {
                case "month":
                    number = (end.Year - start.Year) * 12 + end.Month - start.Month;
                    break;
                case "week":
                    number = GetWeeks(start, end);
                    break;
                case "day":
                    number = (int)(end - start).TotalDays;
                    break;
                case "hour":
                    if ((end - start).TotalDays > 30)
                        throw new ApplicationException("'Hour' interval is only allowed for time spans under 31 days.");
                    number = (int)(end - start).TotalHours;
                    break;
                default:
                    throw new ApplicationException($"Wrong interval ({interval ?? "null"}) for statistical data. Only 'month' 'week' 'day' or 'hour' is allowed.");
            }

            return number;
        }

        private DateTime[] GetIntervalValues(DateTime start, string interval, int intervals)
        {
            DateTime[] dates = new DateTime[intervals];

            for (int i = 0; i < intervals; i++)
            {
                switch (interval)
                {
                    case "month":
                        dates[i] = start.AddMonths(i);
                        break;
                    case "week":
                        DateTime monday = GetStartOfWeek(start);
                        dates[i] = monday.AddDays(7 * i);
                        break;
                    case "day":
                        dates[i] = start.AddDays(i);
                        break;
                    case "hour":
                        dates[i] = start.AddHours(i);
                        break;
                }
            }

            return dates;
        }

        private DateTime GetStartOfWeek(DateTime input)
        {
            int dayOfWeek = (((int)input.DayOfWeek) + 6) % 7;
            return input.Date.AddDays(-dayOfWeek);
        }

        private int GetWeeks(DateTime start, DateTime end)
        {
            start = GetStartOfWeek(start);
            end = GetStartOfWeek(end);
            int days = (int)(end - start).TotalDays;
            return (days / 7);
        }        

        #endregion
    }
}
