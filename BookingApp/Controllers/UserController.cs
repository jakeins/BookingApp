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

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly UserService userService;
        readonly IMapper mapper;
        public UserController(UserService userService)
        {
            this.userService = userService;
            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, UserMinimalDto>();
                cfg.CreateMap<ApplicationUser, UserMinimalDto>().ReverseMap();
                cfg.CreateMap<UserGetMinimalDTO, ApplicationUser>();
                cfg.CreateMap<UserGetMinimalDTO, ApplicationUser>().ReverseMap();

            }));
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateUser([FromBody] UserMinimalDto user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser appUser = mapper.Map<UserMinimalDto, ApplicationUser>(user);
                    await userService.CreateUser(appUser, user.Password);
                    return Ok("User created");
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            return BadRequest("Error valid");
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById([FromRoute] string userId)
        {
            try
            {
                ApplicationUser appuser = await userService.GetUserById(userId);
                UserGetMinimalDTO user = mapper.Map<ApplicationUser, UserGetMinimalDTO>(appuser);
                return new OkObjectResult(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                IEnumerable<ApplicationUser> appusers = await userService.GetUsersList();
                IEnumerable<UserGetMinimalDTO> users = mapper.Map<IEnumerable<ApplicationUser>, IEnumerable<UserGetMinimalDTO>>(appusers);
                return new OkObjectResult(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        [HttpDelete("{userId}")]
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
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser()
        {
            try
            {
                ApplicationUser appuser = await userService.GetUserById("fee4e814-daac-4e64-8e86-e92694da7486");
                appuser.Email = "wolf@user.cow";
                appuser.UserName = "Wolf";
                await userService.UpdateUser(appuser);
                return new OkObjectResult("User updated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("{userId}")]
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
    }
}