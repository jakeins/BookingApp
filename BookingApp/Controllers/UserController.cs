using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Exceptions;
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
    public class UserController : EntityControllerBase
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

        
        [HttpPost("api/user")]
        public async Task<IActionResult> CreateUser([FromBody] AuthRegisterDto user)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
            await userService.CreateUser(appUser, user.Password);
            return Ok("User created");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("api/user/create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] AuthRegisterDto user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
            await userService.CreateAdmin(appUser, user.Password);
            return Ok("User created");       
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpGet("api/user/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute]string userId)
        {
            if (UserId == userId || IsAdmin)
            {
                ApplicationUser appuser = await userService.GetUserById(userId);
                UserMinimalDto user = mapper.Map<ApplicationUser, UserMinimalDto>(appuser);
                return new OkObjectResult(user);
            }
            else
                return BadRequest("Can not get information about this user");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpGet("api/user/email/{userEmail}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute]string userEmail)
        {
            ApplicationUser requestUser = await userService.GetUserByEmail(userEmail);
            if (UserId == requestUser.Id || IsAdmin)
            {
                UserMinimalDto user = mapper.Map<ApplicationUser, UserMinimalDto>(requestUser);
                return new OkObjectResult(user);
            }
            else
                return BadRequest("Can not get information about this user");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            IEnumerable<ApplicationUser> appusers = await userService.GetUsersList();
            IEnumerable<UserMinimalDto> users = mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserMinimalDto>>(appusers);
            return new OkObjectResult(users);
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("api/user/{userId}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] string userId)
        {
            //TODO: Delete all user bookings
            await userService.RemoveAllRolesFromUser(userId);
            await userService.DeleteUser(userId);
            return new OkObjectResult("User deleted");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpPut("api/user/{userId}")]
        public async Task<IActionResult> UpdateUser([FromBody]UserUpdateDTO user, [FromRoute]string userId)
        {
            if (UserId == userId || IsAdmin)
            {
                ApplicationUser appuser = await userService.GetUserById(userId);
                mapper.Map<UserUpdateDTO, ApplicationUser>(user, appuser);
                await userService.UpdateUser(appuser);
                return new OkObjectResult("User updated");
            }
            else
                return BadRequest("Can not update this user");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpGet("api/user/{userId}/roles")]
        public async Task<IActionResult> GetUserRoleById([FromRoute]string userId)
        {
            if (UserId == userId || IsAdmin)
            {
                var userRoles = await userService.GetUserRolesById(userId);
                return new OkObjectResult(userRoles);
            }
            else
                return BadRequest("Can not get roles for this user");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpGet("api/user/{userId}/resources")]
        public async Task<IActionResult> GetResources([FromRoute]string userId)
        {
            if (UserId == userId || IsAdmin)
            {
                var resources = await resourcesService.ListByAssociatedUser(userId);
                var userResources = mapper.Map<IEnumerable<Resource>, IEnumerable<ResourceMaxDto>>(resources);
                return new OkObjectResult(userResources);
            }
            else
                return BadRequest("Can not get resource for this user");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpPut("api/user/{userId}/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody]UserPasswordChangeDTO userDTO, [FromRoute]string userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (UserId == userId)
            {
                await userService.ChangePassword(userId, userDTO.CurrentPassword, userDTO.NewPassword);
                return new OkObjectResult("Password changed");
            }
            else
                return BadRequest("Can not change for this user");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/add-role")]
        public async Task<IActionResult> AddRole([FromRoute]string userId, [FromBody]UserRoleDto roleDto)
        {
            await userService.AddUserRoleAsync(userId, roleDto.Role);
            return Ok("Role added");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole([FromRoute]string userId, [FromBody]UserRoleDto roleDto)
        {
            await userService.RemoveUserRoleAsync(userId, roleDto.Role);
            return Ok("Role removed");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/approval")]
        public async Task<IActionResult> UserApproval([FromRoute]string userId, [FromBody]UserApprovalDto userApprovalDto)
        {
            await userService.UserApproval(userId, userApprovalDto.IsApproved);
            return Ok("User approved");
        }

        [Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user/{userId}/blocking")]
        public async Task<IActionResult> UserBlocking([FromRoute]string userId, [FromBody]UserBlockingDto userBlockingDTO)
        {
            await userService.UserBlocking(userId, userBlockingDTO.IsBlocked);
            return Ok("User blocked");
        }

        [Authorize(Roles = RoleTypes.User)]
        [HttpPut("api/user/{userId}/reset-password")]
        public async Task<IActionResult> ResetPassword([FromRoute]string userId, string token, [FromBody]UserNewPasswordDto userNewPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (UserId == userId)
            {
          
                await userService.ResetUserPassword(userId, token, userNewPasswordDto.NewPassword);
                return new OkObjectResult("Password have been reset");
            }    
            else
                return BadRequest("Can not reset password for this user");
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