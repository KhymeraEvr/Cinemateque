using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemateque.SignalrChat
{
   public class ChatHub : Hub
   {
      public void Send( string name, string message )
      {         
        // Clients.All.broadcastMessage( name, message );
      }
   }
}
