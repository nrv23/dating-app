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
            await _presenceTRacker.UserConnected(Context.User.getUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.getUsername());
            // enviar el evento 'UserIsOnline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado

            var currentUsers = await _presenceTRacker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnLineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            await _presenceTRacker.UserDisconnected(Context.User.getUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOffline", Context.User.getUsername());

            var currentUsers = await _presenceTRacker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnLineUsers", currentUsers);
            // enviar el evento 'UserIsOffline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado
            await base.OnDisconnectedAsync(exception);
        }
    }
}