using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        readonly BookingsService bookingService;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;
        readonly IMapper dtoMapper;

        public BookingsController(BookingsService bookingService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.bookingService = bookingService;
            this.userManager = userManager;
            this.roleManager = roleManager;

            dtoMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Booking, BookingMinimalDTO>();
                cfg.CreateMap<Booking, BookingOwnerDTO>().ReverseMap();
                cfg.CreateMap<Booking, BookingAdminDTO>().ReverseMap();
            }
            ));
        }

        #region CRUD actions

        /// <summary>
        /// Create new booking. POST: api/BookingsControler
        /// </summary>
        /// <param name="item">New <see cref="Booking"/> data</param>
        /// <returns>Http response code</returns>
        /// <response code="201">Success created</response>
        /// <response code="400">Invalid argument</response>
        /// <response code="404">Resources or rule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Create([FromBody] BookingCreateDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int newBookingId = await bookingService.CreateAsync(item, UserId);
            
            return new CreatedResult(
                $"api/BookingControler/{newBookingId}", 
                dtoMapper.Map<BookingOwnerDTO>(await bookingService.GetAsync(newBookingId))
                );
        }

        // Filtered access: Guest/User/Admin
        /// <summary>
        /// Get <see cref="Booking"/> info. GET: api/BookingControler/{bookingId}
        /// </summary>
        /// <param name="bookingId">Id of exist <see cref="Booking"/></param>
        /// <returns>Status code</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{bookingId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Details([FromRoute] int bookingId)
        {
            var models = await bookingService.GetAsync(bookingId);
               
            if (Anonym)
            {
                var dtos = dtoMapper.Map<BookingMinimalDTO>(models);
                return Ok(dtos);
            }
            else if (AdminAccess)
            {
                var dtos = dtoMapper.Map<BookingAdminDTO>(models);
                return Ok(dtos);
            }
            else
            {
                var dtos = dtoMapper.Map<BookingOwnerDTO>(models);
                return Ok(dtos);
            }
        }

        /// <summary>
        /// Update exist <see cref="Booking"/> PUT: api/BookingControler/{bookingId}
        /// </summary>
        /// <param name="bookingId">Id of exist <see cref="Booking"/></param>
        /// <param name="item">New <see cref="Booking"/> data</param>
        /// <returns>Http response code</returns>
        /// <response code="200">Success update</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Internal server error</response>
        [HttpPut("{bookingId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Update([FromRoute] int bookingId, [FromBody] BookingUpdateDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookingData = await bookingService.GetAsync(bookingId);

            if(AdminAccess || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Update(bookingId, item.StartTime, item.EndTime, UserId, item.Note);
                return Ok("Booking data updated succefully");
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can not edit not owned booking");
            }
        }

        /// <summary>
        /// Delete exist <see cref="Booking"/>. DELETE: api/Booking/5
        /// </summary>
        /// <param name="bookingId">Id of exist <see cref="Booking"/> which delete</param>
        /// <returns>Http status code</returns>
        /// <response code="200">Success deleted</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Error. Internal server</response>
        [HttpDelete("{bookingId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Delete([FromRoute] int bookingId)
        {
            var bookingData = await bookingService.GetAsync(bookingId);

            if (AdminAccess || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Delete(bookingId);
                return Ok("Booking delete succefully");
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can delete not owned booking");
            }
        }

        #endregion

        #region Public Extensions

        /// <summary>
        /// Terminate exist <see cref="Booking"/>. DELETE: api/Booking/terminate/5
        /// </summary>
        /// <param name="bookingId">Id of exist <see cref="Booking"/> which terminate</param>
        /// <returns>Http status code</returns>
        /// <response code="200">Success terminate booking</response>
        /// <response code="400">Error. Invalid booking passed</response>
        /// <response code="401">Error. Only admin and owner can update booking data</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Error. Internal server</response>
        [HttpPut("terminate/{bookingId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Terminate([FromRoute] int bookingId)
        {
            var bookingData = await bookingService.GetAsync(bookingId);

            if (AdminAccess || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Terminate(bookingId, UserId);
                return Ok("Booking delete succefully");
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can terminate not owned booking");
            }
            
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Shorthand for checking if current user authorized
        /// </summary>
        bool Anonym => !User.HasClaim(c => c.Type == "uid");
        /// <summary>
        /// Current user identifier
        /// </summary>
        string UserId => User.Claims.Single(c => c.Type == "uid").Value;
        /// <summary>
        /// Shorthand for checking if current user has admin access level
        /// </summary>
        bool AdminAccess => User.IsInRole(RoleTypes.Admin);
        #endregion
    }
}
