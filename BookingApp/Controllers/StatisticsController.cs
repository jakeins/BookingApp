using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services;

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

        [HttpGet("bpr")]
        public async Task<IActionResult> BookingsPerResourceAllTime()
        {
            var resources = await resourcesService.ListIncludingBookings();

            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach(var item in resources)
            {
                result.Add(item.Title, item.Bookings.Count);
            }

            return Ok(result);
        }
    }
}