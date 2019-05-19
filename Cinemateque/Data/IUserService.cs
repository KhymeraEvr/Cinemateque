using Cinemateque.DataAccess.Models;
using Cinemateque.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinemateque.Data
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password, HttpContext ctx);
        int DecodeTokenForClaims(string token, HttpContext ctx);
    }

    public class UserService : IUserService
    {
        private readonly IFilmService _serv;
        private readonly IConfiguration _config;

        public UserService(IFilmService serv, IConfiguration config)
        {
            _config = config;
            _serv = serv;
        }

        public async Task<User> Register(string username, string password)
        {
            var user = new User
            {
                UserName = username,
                Passwrod = password,
                Role = "User"
            };

            var result = await _serv.Context.User.AddAsync(user);
            await _serv.Context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<User> Authenticate(string username, string password, HttpContext ctx)
        {
            var user = _serv.Context.User.SingleOrDefault(x => x.UserName == username && x.Passwrod == password);

            if (user == null)
            {
                user = await Register(username, password);
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                     new Claim(ClaimTypes.Role, user.Role)

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var appIdentity = new ClaimsIdentity(claims);
            ctx.User.AddIdentity(appIdentity);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Passwrod = null;

            return user;
        }

        public int DecodeTokenForClaims(string token, HttpContext ctx)
        {
            var Jtoken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var userId = Convert.ToInt32(Jtoken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };

            var appIdentity = new ClaimsIdentity(claims);
            ctx.User.AddIdentity(appIdentity);
            return userId;
        }
    }
}
