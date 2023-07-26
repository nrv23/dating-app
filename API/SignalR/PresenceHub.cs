using Microsoft.AspNetCore.SignalR;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTRacker _presenceTRacker;
        public PresenceHub(PresenceTRacker presenceTRacker)
        {
            _presenceTRacker = presenceTRacker;
        }
        public override async Task OnConnectedAsync()
        {
           var isOnline =  await _presenceTRacker.UserConnected(Context.User.getUsername(), Context.ConnectionId);
            if(isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User.getUsername());
            // enviar el evento 'UserIsOnline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado

            var currentUsers = await _presenceTRacker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnLineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            var isOffline = await _presenceTRacker.UserDisconnected(Context.User.getUsername(), Context.ConnectionId);
            if(isOffline)  await Clients.Others.SendAsync("UserIsOffline", Context.User.getUsername());
            // enviar el evento 'UserIsOffline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado
            await base.OnDisconnectedAsync(exception);
        }
    }
}