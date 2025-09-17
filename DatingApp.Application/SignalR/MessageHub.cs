using AutoMapper;
using DatingApp.Application.Extensions;
using DatingApp.Data.Repositories;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Application.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;

        private readonly IMapper mapper;

        private readonly IUserRepository userRepository;

        private readonly PresenceTracker tracker;

        private readonly IHubContext<PresenceHub> presence;

        public MessageHub(
            IMessageRepository messageRepository, 
            IMapper mapper, 
            IUserRepository userRepository, 
            PresenceTracker tracker, 
            IHubContext<PresenceHub> presence)
        {
            this.messageRepository = messageRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.tracker = tracker;
            this.presence = presence;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null)
                throw new HubException("HttpContext is null. Cannot extract query params.");

            var otherUsers = httpContext.Request.Query["user"].ToString();
            if (string.IsNullOrEmpty(otherUsers))
                throw new HubException("Query parameter 'user' is missing");

            var currentUser = Context.User.GetUserName();
            if (string.IsNullOrEmpty(currentUser))
                throw new HubException("User identity not found");

            var groupName = GetGroupName(currentUser, otherUsers);

            await AddToGroupWithConnections(Context, groupName);

            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var messages = await messageRepository.GetMessageThread(currentUser, otherUsers);
            await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await RemoveFromMessageGroup(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto model)
        {
            var currentUserName = Context.User.GetUserName();
            if (currentUserName == model.RecipientUserName) throw new HubException("You Can not Send Message to yourfelf");

            var sender = await userRepository.GetUserByUserNameWithPhotos(currentUserName);
            if (sender == null) throw new HubException("Sender Not Found");

            var recipient = await userRepository.GetUserByUserNameWithPhotos(model.RecipientUserName);
            if (recipient == null) throw new HubException("Recipient Not Found");

            var message = new Message()
            {
                SenderId = sender.Id,
                SenderUserName = sender.UserName,
                ReceiverId = recipient.Id,
                ReceiverUserName = recipient.UserName,
                Content = model.Content,
                SenderPhotoUrl = sender?.Photos.FirstOrDefault(p => p.IsMain == true)?.Url
            };
            await messageRepository.AddMessage(message);

            var groupName = GetGroupName(currentUserName, recipient.UserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(u => u.UserName == recipient.UserName))
            {
                message.DateRead = DateTime.Now;
                message.IsRead = true;
            }
            else
            {
                var connectionIds = await tracker.GetConnectionsForUser(recipient.UserName);
                if (connectionIds != null)
                {
                    await presence.Clients.Clients(connectionIds).SendAsync("NewMessageReceived", new { userName = sender?.UserName, content = model.Content });
                }
            }

            if (await messageRepository.SaveAll())
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
        }

        private static string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}--{other}" : $"{other}--{caller}";
        }

        private async Task<bool> AddToGroupWithConnections(HubCallerContext context, string groupName)
        {
            var currentUserName = Context.User?.GetUserName();
            if (string.IsNullOrEmpty(currentUserName))
                throw new HubException("User name is null");

            var connection = new Connection(context.ConnectionId, currentUserName);
            var group = await messageRepository.GetMessageGroup(groupName);

            if (group == null)
            {
                group = new Group(groupName);

                if (group.Connections == null)
                    group.Connections = new List<Connection>();

                group.Connections.Add(connection);
                messageRepository.AddGroup(group);
            }
            else
            {
                if (group.Connections == null)
                    group.Connections = new List<Connection>();

                group.Connections.Add(connection);
            }

            var result = await messageRepository.SaveAll();
            return true;
        }

        private async Task RemoveFromMessageGroup(string connectionId)
        {
            var connection = await messageRepository.GetConnection(connectionId);
            messageRepository.RemoveConnection(connection);
            await messageRepository.SaveAll();
        }
        
    }
}
