using API.DTOs;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Extensions;
using API.Entities;
using API.Helpers;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository messageRepository;
        private readonly IUserRespository userRespository;

        public IMapper _mapper { get; }

        public MessagesController(IMessageRepository messageRepository, IUserRespository userRespository, IMapper mapper)
        {
            this.messageRepository = messageRepository;
            this.userRespository = userRespository;
            this._mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> createMessage(CreateMessageDTO createMessage)
        {

            var username = User.getUsername();
            if (username == createMessage.RecipientUsername) return BadRequest("You cannot send messages to yourself");

            var sender = await userRespository.GetUserByUsernameAsync(username);
            var recipient = await userRespository.GetUserByUsernameAsync(createMessage.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessage.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));
            return BadRequest("Fail to save message");
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)        
        {
            messageParams.UserName = User.getUsername();
            var messages = await messageRepository.GetMessagesForUser(messageParams);
            Console.WriteLine("messages", messages);
            Response.AddPaginationHeader(new PaginationHeader(messages.currentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
            return messages;
        }

        [HttpGet("thread/{username}")]

        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username) {
            var currentUsername = User.getUsername();
            return Ok(await messageRepository.GetMessageThread(currentUsername,username));
        }
    }
}