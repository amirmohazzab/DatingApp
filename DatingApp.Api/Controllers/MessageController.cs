using AutoMapper;
using DatingApp.Api.Errors;
using DatingApp.Application.Extensions;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class MessageController
        (IMessageRepository messageRepository,
        IUserRepository userRepository,
        IMapper mapper) : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto model)
        {
            var currentUser = User.GetUserName();
            if (currentUser == model.RecipientUserName) return BadRequest("You Can not Send Message to yourfelf");

            var sender = await userRepository.GetUserByUserName(currentUser);
            if (sender == null) return BadRequest(new ApiResponse(400, "Sender Not Found"));

            var recipient = await userRepository.GetUserByUserName(model.RecipientUserName);
            if (recipient == null) return BadRequest(new ApiResponse(400, "Recipient Not Found"));

            var message = new Message()
            {
                SenderId = sender.UserId,
                SenderUserName = sender.UserName,
                ReceiverId = recipient.UserId,
                ReceiverUserName = recipient.UserName,
                Content = model.Content
            };
            await messageRepository.AddMessage(message);

            if(await messageRepository.SaveAll())
                return Ok(mapper.Map<Message, MessageDto>(message));

            return BadRequest(new ApiResponse(400, "Message Sent Failed"));
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();

            return Ok(await messageRepository.GetMessageForUser(messageParams));
        }

        [HttpGet("thread/{userName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userName)
        {
            var currentUserName = User.GetUserName();
            return Ok(await messageRepository.GetMessageThread(currentUserName, userName));
        } 
    }
}
