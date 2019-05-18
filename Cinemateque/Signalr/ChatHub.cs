using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemateque.Signalr
{
   public class ChatHub : Hub
   {
      public async Task SendMessage( string user, string message )
      {
         await Clients.All.SendAsync( "ReceiveMessage", user, message );
      }

      public Task JoinRoom( string roomName )
      {
         return Groups.AddToGroupAsync( Context.ConnectionId, roomName );
      }

      public Task LeaveRoom( string roomName )
      {
         return Groups.RemoveFromGroupAsync( Context.ConnectionId, roomName );
      }

      public string GetConnectionId()
      {
         return Context.ConnectionId;
      }
   }
}
