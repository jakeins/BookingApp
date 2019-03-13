using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services;
using BookingApp.Services.Interfaces;
using BookingApp.Entities.Statistics;
using BookingApp.DTOs;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using BookingApp.Controllers.Bases;

namespace BookingApp.Controllers
{
    [Route("api/stats")]
    [ApiController]
    [Authorize(Roles = RoleTypes.Admin)]
    public class StatisticsController : EntityControllerBase
    {
        readonly IStatisticsService statisticsService;
        readonly IMapper dtoMapper;
        
        public StatisticsController(IStatisticsService statisticsService)
        {
            this.statisticsService = statisticsService;
            dtoMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BookingsStats, BookingStatsDTO>();
                cfg.CreateMap<ResourceStats, ResourceStatsDTO>();
            }));
        }

        [HttpGet("bookings-creations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCreations([FromQuery] DateTime? startTime,[FromQuery] DateTime? endTime,[FromQuery] string interval,[FromQuery] int[] resourcesIDs)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCreations(start, end, interval, resourcesIDs);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-cancellations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCancellations([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] resourcesIDs)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCancellations(start, end, interval, resourcesIDs);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-terminations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsTerminations([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] resourcesIDs)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsTerminations(start, end, interval, resourcesIDs);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-completions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCompletions([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] resourcesIDs)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCompletions(start, end, interval, resourcesIDs);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }


        [HttpGet("resources")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]        
        public async Task<IActionResult> ResourcesUsage()
        {
            List<ResourceStatsDTO> dTOs = new List<ResourceStatsDTO>();

            IEnumerable<ResourceStats> resourceStats = await statisticsService.GetResourceStats();

            foreach(var item in resourceStats)
            {
                dTOs.Add(dtoMapper.Map<ResourceStatsDTO>(item));
            }

            return Ok(dTOs);
        }

        [HttpGet("resources/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResourcesUsage([FromRoute] int id)
        {   
            ResourceStats resourceStats = await statisticsService.GetResourceStats(id);

            ResourceStatsDTO dTO = dtoMapper.Map<ResourceStatsDTO>(resourceStats);

            return Ok(dTO);
        }
    }
}