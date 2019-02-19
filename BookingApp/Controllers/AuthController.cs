using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly NotificationService notificationService;
        private readonly UserService userService;
        private readonly JwtService jwtService;
        private readonly IMapper mapper;

        public AuthController(NotificationService notificationService, UserService userService, JwtService jwtService)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.jwtService = jwtService;

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, AuthRegisterDto>().ReverseMap();
            }));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]AuthLoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userService.GetUserByEmail(dto.Email);

            if (user == null || !await userService.CheckPassword(user, dto.Password))
            {
                ModelState.AddModelError("login_failure", "Invalid email or password");
                return BadRequest(ModelState);
            }

            if (!(user.ApprovalStatus ?? false))
            {
                ModelState.AddModelError("login_failure", "Not approved yet");
                return BadRequest(ModelState);
            }

            if (user.IsBlocked ?? false)
            {
                ModelState.AddModelError("login_failure", "Account has been blocked");
                return BadRequest(ModelState);
            }

            var userClaims = await jwtService.GetClaimsAsync(user);
            var accessToken = jwtService.GenerateJwtAccessToken(userClaims);

            return Ok(new { accessToken });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]AuthRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = mapper.Map<ApplicationUser>(dto);
            await userService.CreateUser(user, dto.Password);
            
            //await userService.AddToRoleAsync(user, RoleTypes.User);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("forget")]
        public async Task<IActionResult> Forget([FromBody]AuthMinimalDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userService.GetUserByEmail(dto.Email);
            await notificationService.ForgetPasswordMail(user);

            return Ok();
        }
    }
}