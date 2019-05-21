using Cinemateque.DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cinemateque.Middleware
{
   public class TokenCookieMiddleware
   {
      private readonly RequestDelegate _next;

      public TokenCookieMiddleware( RequestDelegate next )
      {
         _next = next;
      }

      public async Task Invoke( HttpContext context )
      {
         var token = context.Request.Cookies["Token"];
         if ( token != null && token != "undefined" )
         {
            context.Request.Headers.Append( "Authorization", "Bearer " + token );
            var Jtoken = new JwtSecurityTokenHandler().ReadJwtToken( token );
            var userId = Convert.ToInt32( Jtoken.Claims.FirstOrDefault( c => c.Type == "unique_name" )?.Value );
            var role =  Jtoken.Claims.FirstOrDefault( c => c.Type == "role" )?.Value;
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity( CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role );
            foreach ( var claim in claims )
            {
               identity.AddClaim( claim );

            }
            var principal = new ClaimsPrincipal( identity );
            await context.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, principal );
         }
         await _next.Invoke( context );
      }
   }
   public static class TokenMiddlewareExtension
   {
      public static IApplicationBuilder UseCookieValidationMidddleware( this IApplicationBuilder builder )
      {
         return builder.UseMiddleware<TokenCookieMiddleware>();
      }
   }
}
