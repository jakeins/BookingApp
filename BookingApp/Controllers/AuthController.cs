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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly JwtService jwtService;
        private readonly IMapper mapper;

        public AuthController(UserManager<ApplicationUser> userManager, JwtService jwtService)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;

            mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApplicationUser, RegisterDto>().ReverseMap();
            }));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            {
                ModelState.AddModelError("login_failure", "Invalid email or password");
                return BadRequest(ModelState);
            }

            var userClaims = await jwtService.GetClaimsAsync(user);
            var accessToken = jwtService.GenerateJwtAccessToken(userClaims);

            return Ok(new { accessToken });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = mapper.Map<ApplicationUser>(dto);
            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            result = await userManager.AddToRoleAsync(user, RoleTypes.User);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}