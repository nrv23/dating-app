

using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                     .OrderByDescending(x => x.MessageSent)
                     .AsQueryable();
            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.UserName && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.UserName && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null) // los no leidos
            };

            var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider); // mapear la respuesta del query al tipo MessageDTO

            return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername) //traer los mensajes entre dos usuarios
        {
            // mensajes ya guardados
            var query = context.Messages
                 .Where(m => (
                        m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                        && m.Sender.UserName == recipientUsername
                    ) || (
                        m.Recipient.UserName == recipientUsername &&
                        m.Sender.UserName == currentUsername && m.SenderDeleted == false
                    )
                 )
                 .OrderBy(m => m.MessageSent)
                 .AsQueryable();
            // mensajes del usuario actual que no estan leidos
            var unreadMessages = query.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername).ToList(); // obtiene los mensajes no leidos del usuario actual

            if (unreadMessages.Any()) // pregunta si hay mensajes sin leer
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now; // marca la fecha del mensaje leido
                }

                //await context.SaveChangesAsync(); // gvuarda los cambios en la bd
            }

            //return mapper.Map<IEnumerable<MessageDTO>>(messages); // mapea la respuesta a tipo lista MessageDTO y devuelve
            return await query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider).ToListAsync();
        }


        // ---------------------------------------------------------------------------------

        public async Task<Connection> GetConnection(string ConnectionId)
        {
            return await context.Connections.FindAsync(ConnectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection);
        }

        public void AddGroup(Group group)
        {
            context.Groups.Add(group);
        }

        // ---------------------------------------------------------------------------------

        public async Task<Group> getGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
    }
}