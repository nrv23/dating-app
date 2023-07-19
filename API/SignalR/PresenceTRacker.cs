

namespace API.SignalR
{
    public class PresenceTRacker
    {
        private static readonly Dictionary<string, List<string>> OnLineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string username, string connectionId) {
            // agregar la lista de conexiones separadas por el nombre del usuario
            lock(OnLineUsers) {
                if(OnLineUsers.ContainsKey(username)){
                    OnLineUsers[username].Add(connectionId);
                } else {
                    OnLineUsers.Add(username, new List<string> {connectionId});
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string username,string connectionId) { // si un usuario se desconecta, eliminar todas sus conexiones abiertas
            lock(OnLineUsers){
               
               if(!OnLineUsers.ContainsKey(username)) return Task.CompletedTask;

               OnLineUsers[username].Remove(connectionId); // borra la conexion de ese usuario

               if(OnLineUsers[username].Count == 0) { // si el jusuario no tiene mas conexiones, borrar la instancia de ese usuario en la lista de usuario en linea
                OnLineUsers.Remove(username);
               }
            }

            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers() {
            string[] onlineUsers;
            lock(OnLineUsers) {
                onlineUsers = OnLineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }
    }
}