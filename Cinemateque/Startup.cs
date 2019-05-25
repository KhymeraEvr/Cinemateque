﻿using Cinemateque.Data;
using Cinemateque.DataAccess;
using Cinemateque.Middleware;
using Cinemateque.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinemateque
{
   public class Startup
   {
      public Startup( IConfiguration configuration )
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      public void ConfigureServices( IServiceCollection services )
      {
         services.AddAuthentication( CookieAuthenticationDefaults.AuthenticationScheme )
                 .AddCookie();

         services.Configure<CookiePolicyOptions>( options =>
      {
         options.CheckConsentNeeded = context => true;
         options.MinimumSameSitePolicy = SameSiteMode.None;
      } );

         services.AddDbContext<CinematequeContext>( options =>
              options.UseSqlServer( Configuration["DbConnectionString"] ) );

         services.AddScoped<IUserService, UserService>();
         services.AddScoped<IFilmService, FilmService>();
         services.AddScoped<IOrderService, OrderService>();
            services.AddSingleton( Configuration );
         services.AddMvc().SetCompatibilityVersion( CompatibilityVersion.Version_2_1 );
      }

      public void Configure( IApplicationBuilder app, IHostingEnvironment env )
      {
         if ( env.IsDevelopment() )
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseExceptionHandler( "/Home/Error" );
            app.UseHsts();
         }

         app.UseHttpsRedirection();
         app.UseStaticFiles();
         app.UseCookiePolicy();

         app.UseCors( x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader() );
         app.UseAuthentication();
         app.UseCookieValidationMidddleware();
         app.UseMvc( routes =>
          {
             routes.MapRoute(
                   name: "auth",
                   template: "users/authenticate",
                   defaults: new { controller = "Users", action = "Authenticate" }
                   )
                   .MapRoute(
                   name: "authRed",
                   template: "/Account",
                   defaults: new { controller = "Users", action = "Login" }
                   );

              routes.MapRoute(
                   name: "default",
                   template: "{controller=Home}/{action=Index}/{id?}");
          } );
      }
   }
}
