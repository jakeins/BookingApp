using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookingApp.Services;
using BookingApp.Data.Models;
using AutoMapper;
using BookingApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using BookingApp.Helpers;

namespace BookingApp.Controllers
{
    
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly IUserService userService;
        readonly IMapper mapper;
        readonly IResourcesService resourcesService;

        public UserController(IUserService userService, IResourcesService resourcesService)
        {
            this.userService = userService;
            this.resourcesService = resourcesService;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>().ReverseMap().ForMember(dest => dest.PasswordHash,opt => opt.MapFrom(src => src.Password));
                cfg.CreateMap<UserMinimalDto, ApplicationUser>().ReverseMap();
                cfg.CreateMap<UserUpdateDTO, ApplicationUser>().ReverseMap();
                cfg.CreateMap<Resource, ResourceMaxDto>().ReverseMap();
            }));
        }

        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("api/user")]
        public async Task<IActionResult> CreateUser([FromBody] AuthRegisterDto user)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
                await userService.CreateUser(appUser,user.Password);
                return Ok("User created");
            }
            return BadRequest("Error valid");
        }

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
            mapper.Map<UserUpdateDTO, ApplicationUser>(user,appuser);    
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
        public async Task<IActionResult> ChangePassword([FromBody]UserPasswordChangeDTO userDTO,[FromRoute]string userId)
        {
            await userService.ChangePassword(userId, userDTO.CurrentPassword,userDTO.NewPassword);
            return Ok("Password changed");
        }

        [HttpPut("api/user/{userId}/add-role")]
        public async Task<IActionResult> AddRole([FromRoute]string userId,[FromBody]string role)
        {
            await userService.AddUserRoleAsync(userId, role);
            return Ok("Role added");
        }

        [HttpPut("api/user/{userId}/remove-role")]
        public async Task<IActionResult> RemoveRole([FromRoute]string userId, [FromBody]string role)
        {
            await userService.RemoveUserRoleAsync(userId, role);
            return Ok("Role removed");
        }

        [HttpPut("api/user/{userId}/approval")]
        public async Task<IActionResult> UserApproval([FromRoute]string userId, [FromBody]bool IsApproved)
        { 
            await userService.UserApproval(userId, IsApproved);
            return Ok();
        }

        [HttpPut("api/user/{userId}/blocking")]
        public async Task<IActionResult> UserBlocking([FromRoute]string userId, [FromBody]bool IsBlocked)
        {
            await userService.UserBlocking(userId, IsBlocked);
            return Ok();
        }

        [HttpPut("api/user/{userId}/resset-password")]
        public async Task<IActionResult> RessetPassword([FromRoute]string userId, string token, string newPassword)
        {
            await userService.RessetUserPassword(userId, token,newPassword);
            return Ok();
        }

        #region Bookings
        //TODO: List bookings
        #endregion
    }
}