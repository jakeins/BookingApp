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
        readonly UserService userService;
        readonly IMapper mapper;
        readonly ResourcesService resourcesService;
        public UserController(UserService userService, ResourcesService resourcesService)
        {
            this.userService = userService;
            this.resourcesService = resourcesService;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>();
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>().ReverseMap();
                cfg.CreateMap<UserMinimalDto, ApplicationUser>();
                cfg.CreateMap<UserMinimalDto, ApplicationUser>().ReverseMap();
                cfg.CreateMap<UserUpdateDTO, ApplicationUser>();
                cfg.CreateMap<UserUpdateDTO, ApplicationUser>().ReverseMap();
            }));
        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPost("api/сreate")]
        public async Task<IActionResult> CreateUser([FromBody] AuthRegisterDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser appUser = mapper.Map<AuthRegisterDto, ApplicationUser>(user);
                    await userService.CreateUser(appUser);
                    return Ok("User created");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Error valid");
        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/user/{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute]string userId)
        {
            try
            {
                ApplicationUser appuser = await userService.GetUserById(userId);
                UserMinimalDto user = mapper.Map<ApplicationUser, UserMinimalDto>(appuser);
                return new OkObjectResult(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IEnumerable<ApplicationUser> appusers = await userService.GetUsersList();
                IEnumerable<UserMinimalDto> users = mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserMinimalDto>>(appusers);
                return new OkObjectResult(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpDelete("api/user/{userId}")]
        public async Task<IActionResult> DeleteUserById([FromRoute] string userId)
        {
            try
            {
                await userService.DeleteUser(userId);
                return new OkObjectResult("User deleted");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpPut("api/user")]
        public async Task<IActionResult> UpdateUser(UserUpdateDTO user)
        {
            try
            {
                ApplicationUser appuser = await userService.GetUserById(user.Id);
                mapper.Map<UserUpdateDTO, ApplicationUser>(user,appuser);    
                await userService.UpdateUser(appuser);
                return new OkObjectResult("User updated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        //[Authorize(Roles = RoleTypes.Admin)]
        [HttpGet("api/user-roles/{userId}")]
        public async Task<IActionResult> GetUserRoleById([FromRoute]string userId)
        {
            try
            {
                var userRoles = await userService.GetUserRolesById(userId);
                return Ok(userRoles);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("api/user-resources/{userId}")]
        public async Task<IActionResult> GetResources([FromRoute]string userId)
        { 
             var userResources = await resourcesService.ListByAssociatedUser(userId);
             return Ok(userResources);
        }
    }
}