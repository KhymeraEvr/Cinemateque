using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemateque.Data;
using Cinemateque.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinemateque.Controllers
{
    public class UsersController : Controller
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //[AllowAnonymous]
        //[HttpPost("authenticate")]
        //public async Task<IActionResult> Authenticate([FromBody]AuthModel userParam)
        //{
        //    var user =  await _userService.Authenticate(userParam.Username, userParam.Password);

        //    if (user == null)
        //        return BadRequest(new { message = "Username or password is incorrect" });

        //    return Ok(user);
        //}

    }
}