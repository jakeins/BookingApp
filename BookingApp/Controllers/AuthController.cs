using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDto dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //TODO: getting user

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //TODO: creating user

            return Ok();
        }
    }
}