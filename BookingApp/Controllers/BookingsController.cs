using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BookingApp.Controllers.Bases;

namespace BookingApp.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingsController : EntityControllerBase
    {
        private readonly IBookingsService bookingService;
        private readonly IMapper dtoMapper;

        public BookingsController(IBookingsService bookingService)
        {
            this.bookingService = bookingService;

            dtoMapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Booking, BookingMinimalDTO>().ReverseMap();
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

            Booking model = dtoMapper.Map<Booking>(item);
            model.Note = item.Note;
            if (!IsAdmin)
                model.CreatedUserId = UserId;
            else
                model.CreatedUserId = item.CreatedUserId;

            await bookingService.CreateAsync(model);

            return new CreatedResult(
                $"api/booking/{model.Id}",
                dtoMapper.Map<BookingOwnerDTO>(await bookingService.GetAsync(model.Id))
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
            var model = await bookingService.GetAsync(bookingId);

            if (model == null)
                throw new Exceptions.EntryNotFoundException($"Booking with id {bookingId} not found");

            if (IsAdmin)
            {
                var dtos = dtoMapper.Map<BookingAdminDTO>(model);
                return Ok(dtos);
            }
            else if(model.CreatedUserId == UserId)
            {
                var dtos = dtoMapper.Map<BookingOwnerDTO>(model);
                return Ok(dtos);
            }
            else
            {
                var dtos = dtoMapper.Map<BookingMinimalDTO>(model);
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

            if (IsAdmin || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Update(bookingId, item.StartTime, item.EndTime, UserId, item.Note);
                return new OkResult();
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

            if (IsAdmin || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Delete(bookingId);
                return new OkResult();
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can delete not owned booking");
            }
        }

        /// <summary>
        /// Return list of all <see cref="Booking"/>
        /// </summary>
        /// <param name="startTime">Optional start time</param>
        /// <param name="endTime">Optional end time</param>
        /// <returns>List of all <see cref="Booking"/></returns>
        /// <response code="200">Success</response>
        /// <response code="401">Error. Access denied</response>
        /// <respomse code="404">Error. Non exist booking id passed</respomse>
        /// <response code="500">Internal server error</response>
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [HttpGet()]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> GetAll([FromQuery]DateTime? startTime, [FromQuery]DateTime? endTime)
        {
            var bookings = await bookingService.ListBookings(startTime ?? DateTime.Now, endTime??DateTime.MaxValue);

            var dtos = new List<BookingAdminDTO>();

            foreach (var booking in bookings)
            {
                dtos.Add(dtoMapper.Map<BookingAdminDTO>(booking));
            }

            return Ok(dtos);
        }

        #endregion CRUD actions

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

            if (IsAdmin || bookingData.CreatedUserId == UserId)
            {
                await bookingService.Terminate(bookingId, UserId);
                return new OkResult();
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can terminate not owned booking");
            }
        }

        #endregion Public Extensions
    }
}