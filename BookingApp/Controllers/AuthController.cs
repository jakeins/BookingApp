using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]LoginDto dto)
        {
            //TODO: getting user

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterDto dto)
        {
            //TODO: creating user

            return Ok();
        }
    }
}