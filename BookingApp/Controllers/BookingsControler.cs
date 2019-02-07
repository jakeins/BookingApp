using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
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
    public class BookingsControler : ControllerBase
    {
        readonly BookingsService bookingService;
        readonly UserManager<ApplicationUser> userManager;
        readonly RoleManager<IdentityRole> roleManager;
        readonly IMapper dtoMapper;

        public BookingsControler(BookingsService bookingService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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

        // GET: api/BookingsControler/5
        // Filtered access: Guest/User/Admin
        [HttpGet("{resourceId}")]
        public async Task<IActionResult> Index([FromRoute] int resourceId)
        {
            bool adminAccess = await CurrentUserHasRole(RoleTypes.Admin);
            bool userAccess = await CurrentUserHasRole(RoleTypes.User);

            var models = await bookingService.ListBookingOfResource(resourceId, adminAccess);

            if (adminAccess)
            {
                var dtos = dtoMapper.Map<IEnumerable<BookingAdminDTO>>(models);
                return Ok(dtos);
            }
            else if(userAccess)
            {
                var dtos = dtoMapper.Map<IEnumerable<BookingOwnerDTO>>(models);
                return Ok(dtos);
            }
            else
            {
                var dtos = dtoMapper.Map<IEnumerable<BookingMinimalDTO>>(models);
                return Ok(dtos);
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
