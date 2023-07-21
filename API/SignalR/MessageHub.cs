
using Microsoft.AspNetCore.SignalR;
using API.interfaces;
using API.Extensions;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRespository userRespository;
        private readonly IMapper _mapper;

        public MessageHub(IMessageRepository Mrepository, IUserRespository Urespository, IMapper mapper)
        {
            this.messageRepository = Mrepository;
            this.userRespository = Urespository;
            this._mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var gruopName = GetGroupName(Context.User.getUsername(), otherUser);

            // agregar los usuarios al grupo
            await Groups.AddToGroupAsync(Context.ConnectionId, gruopName);
            await AddToGroup(gruopName);

            // obtener los mensajes 
            var messages = await this.messageRepository.GetMessageThread(Context.User.getUsername(), otherUser);

            // retorna los mensajes  
            await Clients.Group(gruopName).SendAsync("ReceivedMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }
        private async Task<bool> AddToGroup(string gruopName){
            var group = await messageRepository.GetMessageGroup(gruopName);
            var connection = new Connection(Context.ConnectionId, Context.User.getUsername());

            if(group == null ) {
                group = new Group(gruopName);
                messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            return await messageRepository.SaveAllAsync();
        } 

        private async Task RemoveFromMessageGroup() {
            var connection = await messageRepository.GetConnection(Context.ConnectionId);
            messageRepository.RemoveConnection(connection);
            await messageRepository.SaveAllAsync();
        }

        public async Task SendMessage(CreateMessageDTO createMessage)
        {

    
            var username = Context.User.getUsername();
            if (username == createMessage.RecipientUsername) throw new HubException("You cannot send messages to yourself");

            var sender = await userRespository.GetUserByUsernameAsync(username);
            var recipient = await userRespository.GetUserByUsernameAsync(createMessage.RecipientUsername);

            if (recipient == null) throw new HubException("user not found");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessage.Content
            };

            var gruopName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(gruopName);

            if(group.Connections.Any(x => x.UserName == recipient.UserName)){ // marca el mensaje como leido
                message.DateRead = DateTime.UtcNow;
            }

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) {
                await Clients.Group(gruopName).SendAsync("NewMessage",_mapper.Map<MessageDTO>(message));
            } else {
                throw new HubException("Fail to save message");
            }
            
        }
    }
}