using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingApp.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IResourcesService resourcesService;
        private readonly IBookingsService bookingsService;

        public UserController(IUserService userService, IResourcesService resourcesService, IBookingsService bookingsService)
        {
            this.userService = userService;
            this.resourcesService = resourcesService;
            this.bookingsService = bookingsService;

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>().ReverseMap().ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
                cfg.CreateMap<UserMinimalDto, ApplicationUser>().ReverseMap();
                cfg.CreateMap<UserUpdateDTO, ApplicationUser>().ReverseMap();
                cfg.CreateMap<Resource, ResourceMaxDto>().ReverseMap();
                cfg.CreateMap<Booking, BookingOwnerDTO>();
            }));
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("api/user")]
        public async Task<IActionResult> CreateUser([FromBody] AuthRegisterDto user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
                await userService.CreateUser(appUser, user.Password);
                return Ok("User created");
            }
            return BadRequest("Error valid");
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("api/user/create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AuthRegisterDto user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
                await userService.CreateUser(appUser, user.Password);
                return Ok("User created");
            }
            return BadRequest("Error valid");
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/user/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute]string userId)
        {
            ApplicationUser appuser = await userService.GetUserById(userId);
            UserMinimalDto user = mapper.Map<ApplicationUser, UserMinimalDto>(appuser);
            return new OkObjectResult(user);
        }

        [HttpGet("api/user/email/{userEmail}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute]string userEmail)
        {
            ApplicationUser appuser = await userService.GetUserByEmail(userEmail);
            UserMinimalDto user = mapper.Map<ApplicationUser, UserMinimalDto>(appuser);
            return new OkObjectResult(user);
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<ApplicationUser> appusers = await userService.GetUsersList();
            IEnumerable<UserMinimalDto> users = mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserMinimalDto>>(appusers);
            return new OkObjectResult(users);
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("api/user/{userId}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] string userId)
        {
            await userService.DeleteUser(userId);
            return new OkObjectResult("User deleted");
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody]UserUpdateDTO user, [FromRoute]string userId)
        {
            ApplicationUser appuser = await userService.GetUserById(userId);
            mapper.Map<UserUpdateDTO, ApplicationUser>(user, appuser);
            await userService.UpdateUser(appuser);
            return new OkObjectResult("User updated");
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/user/{userId}/roles")]
        public async Task<IActionResult> GetUserRoleById([FromRoute]string userId)
        {
            var userRoles = await userService.GetUserRolesById(userId);
            return Ok(userRoles);
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/user/{userId}/resources")]
        public async Task<IActionResult> GetResources([FromRoute]string userId)
        {
            var resources = await resourcesService.ListByAssociatedUser(userId);
            var userResources = mapper.Map<IEnumerable<Resource>, IEnumerable<ResourceMaxDto>>(resources);
            return Ok(userResources);
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]UserPasswordChangeDTO userDTO, [FromRoute]string userId)
        {
            if (ModelState.IsValid)
            {
                await userService.ChangePassword(userId, userDTO.CurrentPassword, userDTO.NewPassword);
                return Ok("Password changed");
            }
            return BadRequest("Model is not valid");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/add-role")]
        public async Task<IActionResult> AddRole([FromRoute]string userId, [FromBody]UserRoleDto roleDto)
        {
            await userService.AddUserRoleAsync(userId, roleDto.Role);
            return Ok("Role added");
        }

        [HttpPut("api/user/{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole([FromRoute]string userId, [FromBody]UserRoleDto roleDto)
        {
            await userService.RemoveUserRoleAsync(userId, roleDto.Role);
            return Ok("Role removed");
        }

        [HttpPut("api/user/{userId}/approval")]
        public async Task<IActionResult> UserApproval([FromRoute]string userId, [FromBody]UserApprovalDto userApprovalDto)
        {
            await userService.UserApproval(userId, userApprovalDto.IsApproved);
            return Ok("User approved");
        }

        [HttpPut("api/user/{userId}/blocking")]
        public async Task<IActionResult> UserBlocking([FromRoute]string userId, [FromBody]UserBlockingDto userBlockingDTO)
        {
            await userService.UserBlocking(userId, userBlockingDTO.IsBlocked);
            return Ok("User blocked");
        }

        [HttpPut("api/user/{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword([FromRoute]string userId, string token, [FromBody]UserNewPasswordDto userNewPasswordDto)
        {
            if (ModelState.IsValid)
            {
                await userService.ResetUserPassword(userId, token, userNewPasswordDto.NewPassword);
                return Ok();
            }
            return BadRequest("Model is not valid");
        }

        #region Bookings

        /// <summary>
        /// Returns user bookings in selected time range
        /// </summary>
        /// <param name="userId">Id of <see cref="ApplicationUser"/></param>
        /// <param name="startTime">Optional start time, if not setted then use current server time</param>
        /// <param name="endTime">Optional end time, if not setted then return all booking from startTime</param>
        /// <returns>List of <see cref="BookingOwnerDTO"/></returns>
        [HttpGet("api/user/{userId}/bookings")]
        public async Task<IActionResult> GetBookings([FromRoute]string userId, [FromQuery]DateTime? startTime, [FromQuery]DateTime? endTime)
        {
            var bookings = await bookingsService.ListBookingForSpecificUser(userId, startTime ?? DateTime.Now, endTime ?? DateTime.MaxValue);
            var dtos = new List<BookingOwnerDTO>();

            foreach (var booking in bookings)
            {
                dtos.Add(mapper.Map<BookingOwnerDTO>(booking));
            }

            return Ok(dtos);
        }

        #endregion Bookings
    }
}