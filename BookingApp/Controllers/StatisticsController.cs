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
    [Authorize(Roles = RoleTypes.Admin)]
    public class StatisticsController : EntityControllerBase
    {
        readonly IResourcesService resourcesService;

        IEnumerable<Resource> resources;
        
        public StatisticsController(IResourcesService service)
        {
            resourcesService = service;            
        }
        

        [HttpGet("resources")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]        
        public async Task<IActionResult> ResourcesUsage()
        {
            resources = await resourcesService.ListIncludingBookingsAndRules();

            List<ResourceUsageDTO> dTOs = new List<ResourceUsageDTO>();            

            foreach(var resource in resources)
            {
                ResourceUsageDTO dTO = new ResourceUsageDTO();
                List<long> timeSpansinTicks = new List<long>();
                double cancellations=0;

                foreach (var booking in resource.Bookings)
                {
                    if(booking.TerminationTime==null||booking.TerminationTime>booking.StartTime)
                    {
                        timeSpansinTicks.Add((booking.EndTime - booking.StartTime).Ticks);
                    }
                    else
                    {
                        cancellations++;
                    }
                }
                
                long longAverageTicks = (long)timeSpansinTicks.Average();
                long minTicks = timeSpansinTicks.Min();
                long maxTicks = timeSpansinTicks.Max();
                long modeTicks = timeSpansinTicks.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();

                long maxRuleTime = new TimeSpan(0, resource.Rule.MaxTime.GetValueOrDefault(),0).Ticks;

                dTO.Title = resource.Title;
                dTO.AverageTime = new TimeSpan(longAverageTicks);
                dTO.MinTime = new TimeSpan(minTicks);
                dTO.MaxTime = new TimeSpan(maxTicks);
                dTO.ModeTime = new TimeSpan(modeTicks);
                dTO.CancellationRate = cancellations / resource.Bookings.Count();
                dTO.AverageUsageRate = (double)longAverageTicks / maxRuleTime;
                dTOs.Add(dTO);
            }

            return Ok(dTOs);
        }
    }
}