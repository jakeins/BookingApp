using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services.Interfaces;
using BookingApp.Entities.Statistics;
using BookingApp.DTOs.Statistics;
using BookingApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Controllers.Bases;

namespace BookingApp.Controllers
{
    [Route("api/stats")]
    [ApiController]
    [Authorize(Roles = RoleTypes.Admin)]
    public class StatisticsController : EntityControllerBase
    {
        readonly IStatisticsService statisticsService;
        readonly IMapperService dtoMapper;
        
        public StatisticsController(IStatisticsService statisticsService, IMapperService mapperService)
        {
            this.statisticsService = statisticsService;
            dtoMapper = mapperService;
        }

        [HttpGet("bookings-creations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCreations([FromQuery] DateTime? startTime,[FromQuery] DateTime? endTime,[FromQuery] string interval,[FromQuery] int[] rID)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCreations(start, end, interval, rID);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-cancellations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCancellations([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] rID)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCancellations(start, end, interval, rID);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-terminations")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsTerminations([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] rID)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsTerminations(start, end, interval, rID);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }

        [HttpGet("bookings-completions")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> BookingsCompletions([FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string interval, [FromQuery] int[] rID)
        {
            DateTime start = startTime ?? DateTime.Now.AddDays(-7);
            DateTime end = endTime ?? DateTime.Now;
            BookingsStats stats = await statisticsService.GetBookingsCompletions(start, end, interval, rID);
            BookingStatsDTO dto = dtoMapper.Map<BookingStatsDTO>(stats);
            return Ok(dto);
        }


        [HttpGet("resources")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]        
        public async Task<IActionResult> ResourcesUsage()
        {
            IEnumerable<ResourceStatsDTO> dTOs;

            IEnumerable<ResourceStats> resourceStats = await statisticsService.GetResourceStats();

            dTOs = dtoMapper.Map<IEnumerable<ResourceStatsDTO>>(resourceStats);

            return Ok(dTOs);
        }

        [HttpGet("resources-rating")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ResourcesRating()
        {
            IEnumerable<ResourceStatsBriefDTO> dTOs;

            IEnumerable<ResourceStats> resourceStats = await statisticsService.GetResourcesRating();

            dTOs = dtoMapper.Map<IEnumerable<ResourceStatsBriefDTO>>(resourceStats);

            return Ok(dTOs);
        }

        [HttpGet("resources/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ResourceUsage([FromRoute] int id)
        {   
            ResourceStats resourceStats = await statisticsService.GetResourceStats(id);

            ResourceStatsDTO dTO = dtoMapper.Map<ResourceStatsDTO>(resourceStats);

            return Ok(dTO);
        }

        [HttpGet("users")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UsersStats()
        {
            UsersStatsDTO dTO;

            UsersStats userStats = await statisticsService.GetUsersStats();

            dTO = dtoMapper.Map<UsersStatsDTO>(userStats);

            return Ok(dTO);
        }
    }
}