using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void DeleteMessage(int messageId)
        {
            throw new NotImplementedException();
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
            var messages = await dbContext.Messages
                .Where(p => p.ReceiverUserName == currentUserName && p.SenderUserName == recipientUserName ||
                 p.SenderUserName == currentUserName && p.ReceiverUserName == recipientUserName)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .OrderBy(p => p.MessageSent).ToListAsync();

            await UpdateMessageToRead(messages, currentUserName);
            return messages;
        }

        public async Task<bool> SaveAll()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task UpdateMessageToRead(List<MessageDto> messages, string userName)
        {
            messages = messages.Where(p => p?.DateRead == null && p.ReceiverUserName == userName).ToList();
            if (messages.Any())
            {
                messages.ForEach(p =>
                {
                    p.DateRead = DateTime.Now;
                    p.IsRead = true;
                });
                dbContext.UpdateRange(mapper.Map<List<Message>>(messages));
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
