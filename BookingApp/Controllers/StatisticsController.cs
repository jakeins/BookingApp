using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services;
using BookingApp.DTOs;

namespace BookingApp.Controllers
{
    [Route("api/stats")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        readonly IResourcesService resourcesService;

        public StatisticsController(IResourcesService service)
        {
            resourcesService = service;
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> BookingsPerResource([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval)
        {
            DateTime start = startTime ?? DateTime.Now.AddYears(-1);
            DateTime end = endTime ?? DateTime.Now;

            var resources = await resourcesService.ListIncludingBookings();

            var result = new List<BookingsPerResourceAllDTO>();

            int intervalsNumber = GetIntervalsNumber(start, end, interval)+1;            

            foreach(var resource in resources)
            {
                BookingsPerResourceAllDTO dto = new BookingsPerResourceAllDTO
                {
                    ResourceTitle = resource.Title,
                    BookingsPerInterval = new int[intervalsNumber]
                };

                foreach (var booking in resource.Bookings)
                {
                    if(booking.CreatedTime>=start&&booking.CreatedTime<=end)
                    {
                        int intervalID = GetIntervalsNumber(start, booking.CreatedTime, interval);
                        dto.BookingsPerInterval[intervalID]++;
                    }
                }

                dto.BookingsSum = dto.BookingsPerInterval.Sum();

                result.Add(dto);
            }
            
            return Ok(result);
        }

        #region Helpers

        private static int GetIntervalsNumber(DateTime start, DateTime end, string interval)
        {
            int number = 0;

            switch (interval)
            {
                case "month":
                    number = (int)(end - start).TotalDays / 30;
                    break;
                case "week":
                    number = (int)(end - start).TotalDays / 7;
                    break;
                case "day":
                    number = (int)(end - start).TotalDays;
                    break;
                case "hour":
                    number = (int)(end - start).TotalHours;
                    break;
                default:
                    // error
                    break;
            }

            return number;
        }

        #endregion
    }
}