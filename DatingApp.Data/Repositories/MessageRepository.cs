using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Connection = DatingApp.Domain.Entities.User.Connection;

namespace DatingApp.Data.Repositories
{
    public class MessageRepository 
        (DatingAppDbContext dbContext,
        IMapper mapper): IMessageRepository
    {
        public async Task AddMessage(Message message)
        {
            await dbContext.Messages.AddAsync(message);
        }

        public void DeleteMessage(Message message)
        {
            dbContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessageById(int messageId)
        {
            return await dbContext.Messages.FindAsync(messageId);
        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = dbContext.Messages.OrderByDescending(p => p.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(p => p.ReceiverUserName == messageParams.UserName),
                "Outbox" => query.Where(p => p.SenderUserName == messageParams.UserName),
                _ => query.Where(p => p.ReceiverUserName == messageParams.UserName && p.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messageEntities = await dbContext.Messages
                .Where(p => p.ReceiverUserName == currentUserName && p.SenderUserName == recipientUserName ||
                 p.SenderUserName == currentUserName && p.ReceiverUserName == recipientUserName)
                .OrderBy(p => p.MessageSent).ToListAsync();

            await UpdateMessageToRead(messageEntities, currentUserName);
            var messages = mapper.Map<IEnumerable<MessageDto>>(messageEntities);
            return messages;
        }

        public async Task<bool> SaveAll()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task UpdateMessageToRead(List<Message> messages, string currentUserName)
        {
            if (messages == null) return;

            var unreadMessages = messages.Where(p => p?.DateRead == null && p.ReceiverUserName == currentUserName).ToList();
            if (!unreadMessages.Any()) return;

            if (unreadMessages.Any())
            {
                unreadMessages.ForEach(p =>
                {
                    p.DateRead = DateTime.Now;
                    p.IsRead = true;
                });
                //dbContext.UpdateRange(mapper.Map<List<Message>>(messages));
                await dbContext.SaveChangesAsync();
            }
        }

        #region Signalr

        public void AddGroup(Group group)
        {
            dbContext.Groups.Add(group);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await dbContext.Connections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            if (dbContext == null)
                throw new Exception("dbContext is null!");

            if (dbContext.Groups == null)
                throw new Exception("dbContext.Groups is null!");

            return await dbContext.Groups.Include(g => g.Connections).FirstOrDefaultAsync(c => c.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
            dbContext.Connections.Remove(connection);
        }

        #endregion
    }
}
