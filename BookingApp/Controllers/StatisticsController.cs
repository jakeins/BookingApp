using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace BookingApp.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatisticsController : EntityControllerBase
    {
        readonly IResourcesService resourcesService;

        IEnumerable<Resource> resources;
        
        public StatisticsController(IResourcesService service)
        {
            resourcesService = service;            
        }

        [HttpGet("bookings")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> BookingsPerResource(
            [FromQuery] DateTime? startTime, 
            [FromQuery] DateTime? endTime, 
            [FromQuery] string interval, 
            [FromQuery] bool checkStatus=false)
        {
            DateTime start = startTime ?? DateTime.Now.AddYears(-1);
            DateTime end = endTime ?? DateTime.Now;

            resources = await resourcesService.ListIncludingBookings();

            int intervals = GetIntervalsNumber(start, end, interval) + 1;            

            if (checkStatus)
            {
                List<BookingsPerResourceBaseDTO> stats = GetStatusDTOs(start, end, interval, intervals);
                return Ok(new BookingStatsDTO(stats,GetIntervalValues(start,interval,intervals)));
            }
            else
            {
                List<BookingsPerResourceBaseDTO> stats = GetBaseDTOs(start, end, interval, intervals);
                return Ok(new BookingStatsDTO(stats, GetIntervalValues(start, interval, intervals)));
            }  
        }

        #region Helpers

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
                    throw new ApplicationException($"Wrong interval ({interval}) for statistical data." +
                        "\nOnly 'month' 'week' 'day' or 'hour'is allowed");
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
                        dates[i] = monday.AddDays(7*i);
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

        private List<BookingsPerResourceBaseDTO> GetBaseDTOs(DateTime start,DateTime end,string interval, int intervals)
        {
            var result = new List<BookingsPerResourceBaseDTO>();            

            foreach (var resource in resources)
            {
                BookingsPerResourceBaseDTO dto = new BookingsPerResourceBaseDTO(resource.Title, intervals);

                foreach (var booking in resource.Bookings)
                {
                    if (booking.CreatedTime >= start && booking.CreatedTime <= end)
                    {
                        int intervalID = GetIntervalsNumber(start, booking.CreatedTime, interval);
                        dto.BookingsPerInterval[intervalID]++;                        
                    }
                }

                dto.BookingsSum = dto.BookingsPerInterval.Sum();

                result.Add(dto);
            }

            return result;
        }

        private List<BookingsPerResourceBaseDTO> GetStatusDTOs(DateTime start, DateTime end, string interval, int intervals)
        {
            var result = new List<BookingsPerResourceBaseDTO>();

            foreach (var resource in resources)
            {
                BookingsPerResourceStatusDTO dto = new BookingsPerResourceStatusDTO(resource.Title, intervals);

                foreach (var booking in resource.Bookings)
                {
                    if (booking.CreatedTime >= start && booking.CreatedTime <= end)
                    {
                        int intervalID = GetIntervalsNumber(start, booking.CreatedTime, interval);
                        dto.BookingsPerInterval[intervalID]++;

                        if (booking.TerminationTime != null)
                        {
                            if (booking.TerminationTime < booking.StartTime)
                            {
                                dto.CancelledBookingsPerInterval[intervalID]++;
                            }
                            else
                            {
                                dto.EarlyTerminatedBookingsPerInterval[intervalID]++;
                            }
                        }
                        else
                        {
                            dto.GoodBookingsPerInterval[intervalID]++;
                        }
                    }
                }

                dto.BookingsSum = dto.BookingsPerInterval.Sum();
                dto.GoodBookingsSum = dto.GoodBookingsPerInterval.Sum();
                dto.EarlyTerminatedBookingsSum = dto.EarlyTerminatedBookingsPerInterval.Sum();
                dto.CancelledBookingsSum = dto.EarlyTerminatedBookingsPerInterval.Sum();

                result.Add(dto);
            }

            return result;
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