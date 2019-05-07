using Cinemateque.Data;
using Cinemateque.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinemateque.Controllers
{
   public class UsersController : Controller
   {
      private IUserService _userService;

      public UsersController( IUserService userService )
      {
         _userService = userService;
      }

      [HttpPost( "authenticate" )]
      public async Task<IActionResult> Authenticate( [FromForm]AuthModel userParam )
      {
         var user = await _userService.Authenticate( userParam.Username, userParam.Password, this.HttpContext );

         if ( user == null )
            return BadRequest( new { message = "Username or password is incorrect" } );
            return RedirectToAction("Index", "Home");
      }

      [HttpGet( "Login" )]
      public IActionResult Login()
      {
         return View();
      }

      [HttpGet( "Logout" )]
      public async Task<IActionResult> Logout()
      {
         await HttpContext.SignOutAsync( CookieAuthenticationDefaults.AuthenticationScheme );

         return RedirectToAction( "Login" );
      }

   }
}