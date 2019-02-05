using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingApp.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody]LoginDTO dto)
        {
            //TODO: getting user

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterDTO dto)
        {
            //TODO: creating user

            return Ok();
        }
    }
}