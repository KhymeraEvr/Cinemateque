using Cinemateque.Models;
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
        User Authenticate(string username, string password);
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

        public User Authenticate(string username, string password)
        {
            var user = _serv.Context.User.SingleOrDefault(x => x.UserName == username && x.Passwrod == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
           // user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Passwrod = null;

            return user;
        }
    }
}
