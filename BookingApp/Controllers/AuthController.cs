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
using BookingApp.Exceptions;

namespace BookingApp.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly INotificationService notificationService;
        private readonly IUserService userService;
        private readonly IJwtService jwtService;
        private readonly IMapperService mapper;

        public AuthController(INotificationService notificationService, IUserService userService, IJwtService jwtService, IMapperService mapper)
        {
            this.notificationService = notificationService;
            this.userService = userService;
            this.jwtService = jwtService;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]AuthLoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = null;
            bool userNotFound = false;
            try
            {
                user = await userService.GetUserByEmail(dto.Email);
            }
            catch (CurrentEntryNotFoundException)
            {
                userNotFound = true;
            }            

            if (userNotFound || !await userService.CheckPassword(user, dto.Password))
            {
                ModelState.AddModelError("loginFailure", "Invalid email or password");
                return BadRequest(ModelState);
            }

            if (!(user.ApprovalStatus ?? false))
            {
                ModelState.AddModelError("loginFailure", "Not approved yet");
                return BadRequest(ModelState);
            }

            if (user.IsBlocked ?? false)
            {
                ModelState.AddModelError("loginFailure", "Account has been blocked");
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

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody]AuthTokensDto dto)
        {
            var principal = jwtService.GetPrincipalFromExpiredAccessToken(dto.AccessToken);
            await jwtService.DeleteRefreshTokenAsync(principal);

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