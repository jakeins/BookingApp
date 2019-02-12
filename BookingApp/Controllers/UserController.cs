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
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>().ReverseMap();
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
                await userService.CreateUser(appUser);
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
        public async Task<IActionResult> UpdateUser(UserUpdateDTO user,string userId)
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
        [HttpGet("api/user/{userId}/resources")]
        public async Task<IActionResult> GetResources([FromRoute]string userId)
        { 
            var resources = await resourcesService.ListByAssociatedUser(userId);
            var userResources = mapper.Map<IEnumerable<Resource>, IEnumerable<ResourceMaxDto>>(resources);
            return Ok(userResources);
        }
    }
}