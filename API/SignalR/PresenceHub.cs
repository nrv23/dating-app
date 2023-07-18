using Microsoft.AspNetCore.SignalR;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.getUsername());
            // enviar el evento 'UserIsOnline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.getUsername());
            // enviar el evento 'UserIsOffline' que indica que alguien se conecto a todos los usuarios menos al usuario actual conectado
            await base.OnDisconnectedAsync(exception);
        }
    }
}