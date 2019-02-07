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

        // POST: api/BookingsControler
        [HttpPost]
        //[Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Create([FromBody] BookingOwnerDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModel = dtoMapper.Map<Booking>(item);
            itemModel.BookingId = 0;
            itemModel.UpdatedUserId = itemModel.CreatedUserId = await GetCurrentUserId();

            await bookingService.CreateAsync(itemModel);

            return Ok("Booking created successfully.");
        }

        // GET: api/BookingsControler/5
        // Filtered access: Guest/User/Admin
        [HttpGet("{bookingId}")]
        public async Task<IActionResult> Details([FromRoute] int bookingId)
        {
            bool adminAccess = await CurrentUserHasRole(RoleTypes.Admin);
            

            var models = await bookingService.GetAsync(bookingId);

            bool onwerAcess = 
                await CurrentUserHasRole(RoleTypes.User) && 
                await GetCurrentUserId() == models.CreatedUserId;

            if (adminAccess)
            {
                var dtos = dtoMapper.Map<BookingAdminDTO>(models);
                return Ok(dtos);
            }
            else if(onwerAcess)
            {
                var dtos = dtoMapper.Map<BookingOwnerDTO>(models);
                return Ok(dtos);
            }
            else
            {
                var dtos = dtoMapper.Map<BookingMinimalDTO>(models);
                return Ok(dtos);
            }
        }

        // PUT: api/Booking/5
        [HttpPut("{bookingId}")]
        //[Authorize(Roles = RoleTypes.User)]
        public async Task<IActionResult> Update([FromRoute] int bookingId, [FromBody] BookingOwnerDTO item)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModel = dtoMapper.Map<Booking>(item);

            var bookingData = await bookingService.GetAsync(bookingId);

            if((await GetCurrentUserRoles()).Contains(Helpers.RoleTypes.Admin) || bookingData.CreatedUserId == await GetCurrentUserId())
            {
                await bookingService.Update(bookingId, item.StartTime, item.EndTime, await GetCurrentUserId(), item.Note);
                return Ok("Booking data updated succefully");
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can not edit not owned booking");
            }
        }

        // DELETE: api/Booking/5
        [HttpDelete("{bookingId}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int bookingId)
        {
            var bookingData = await bookingService.GetAsync(bookingId);

            if ((await GetCurrentUserRoles()).Contains(Helpers.RoleTypes.Admin) || bookingData.CreatedUserId == await GetCurrentUserId())
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

        // DELETE: api/Booking/5
        [HttpDelete("{bookingId}")]
        //[Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> Terminate([FromRoute] int bookingId)
        {
            var bookingData = await bookingService.GetAsync(bookingId);

            if ((await GetCurrentUserRoles()).Contains(Helpers.RoleTypes.Admin) || bookingData.CreatedUserId == await GetCurrentUserId())
            {
                await bookingService.Terminate(bookingId, await GetCurrentUserId());
                return Ok("Booking delete succefully");
            }
            else
            {
                throw new Exceptions.OperationRestrictedException("Can terminate not owned booking");
            }
        }

        #endregion

        #region Private Utils
        async Task<ApplicationUser> GetCurrentUserMOCK() => await userManager.FindByNameAsync("SuperAdmin");

        async Task<string> GetCurrentUserId() => (await GetCurrentUserMOCK()).Id;
        async Task<IEnumerable<string>> GetCurrentUserRoles()
        {
            var result = new List<string>();
            var currentUser = await GetCurrentUserMOCK();

            foreach (var roleName in new[] { RoleTypes.User, RoleTypes.Admin })
                if (await userManager.IsInRoleAsync(currentUser, roleName))
                    result.Add(roleName);

            return result;
        }
        async Task<bool> CurrentUserHasRole(string role) => (await GetCurrentUserRoles()).Contains(role);
        #endregion
    }
}
