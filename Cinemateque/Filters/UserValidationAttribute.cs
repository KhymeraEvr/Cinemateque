using Cinemateque.Data;
using Cinemateque.DataAccess.Models;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cinemateque.Filters
{
   public class UserValidationAttribute : ValidationAttribute
   {
      private IFilmService _serv;

      protected override ValidationResult IsValid( object value, ValidationContext validationContext )
      {
         _serv = validationContext.GetService<IFilmService>();
         var user = (User)validationContext.ObjectInstance;

         if( _serv.Context.User.Any( u => u.UserName  == user.UserName) )
         {
            return new ValidationResult( "This Email is already taken" );
         }

         return ValidationResult.Success;
      }
   }
}
