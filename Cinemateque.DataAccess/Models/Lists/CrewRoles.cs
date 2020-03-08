using System;
using System.Collections.Generic;
using System.Text;

namespace Cinemateque.DataAccess.Models.Lists
{
   public static class CrewRoles
   {
      public static readonly Dictionary<CrewRole, string> Roles = new Dictionary<CrewRole, string>
      {
         {CrewRole.Producer, "Producer" },
         {CrewRole.Screenplay, "Screenplay" },
         {CrewRole.Director, "Director" },
         {CrewRole.OriginalMusicComposer, "Original Music Composer" },
         {CrewRole.DirectorOfPhotography, "Director of Photography" },
         {CrewRole.Writer, "Writer" },
         {CrewRole.CameraOperator, "Camera Operator" },
         {CrewRole.ExecutiveProducer, "Executive Producer" },
         {CrewRole.StillPhotographer, "Still Photographer" },
         {CrewRole.Producer, "Producer" }
      };

      public static readonly HashSet<string> SupportedRoles = new HashSet<string>
      {
         "Producer" ,
         "Screenplay" ,
         "Director" ,
         "Original Music Composer" ,
         "Director of Photography" ,
         "Writer" ,
         "Camera Operator" ,
         "Executive Producer" ,
         "Still Photographer" ,
         "Producer"
      };
}

   public enum CrewRole
   {
      Director,
      Producer,
      Screenplay,
      OriginalMusicComposer,
      DirectorOfPhotography,
      Writer,
      CameraOperator,
      ExecutiveProducer,
      StillPhotographer
   }
}
