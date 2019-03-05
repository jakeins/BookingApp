using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookingApp.Data.Models;
using BookingApp.DTOs;
using BookingApp.Helpers;
using BookingApp.Services.Interfaces;
using BookingApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;
        private readonly IJwtService jwtService;
        private readonly IMapper mapper;

        public AuthController(INotificationService notificationService, IUserService userService, IJwtService jwtService)
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

            if (!await userService.CheckPassword(user, dto.Password))
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
            var refreshToken = jwtService.GenerateJwtRefreshToken();
            await jwtService.LoginByRefreshTokenAsync(user.Id, refreshToken);
            var tokens = new AuthTokensDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpireOn = jwtService.ExpirationTime
            };

            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await jwtService.DeleteRefreshTokenAsync(User);

            return Ok();
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

            await userService.AddUserRoleAsync(user.Id, RoleTypes.User);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody]AuthTokensDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var principal = jwtService.GetPrincipalFromExpiredAccessToken(dto.AccessToken);
            dto.AccessToken = jwtService.GenerateJwtAccessToken(principal.Claims);
            dto.RefreshToken = await jwtService.UpdateRefreshTokenAsync(dto.RefreshToken, principal);
            dto.ExpireOn = jwtService.ExpirationTime;

            return Ok(dto);
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