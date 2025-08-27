using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);

        Task<Message> GetMessageById(int messageId);

        void DeleteMessage(int messageId);

        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);

        Task<bool> SaveAll();

        Task UpdateMessageToRead(List<MessageDto> messages, string userName);
    }
}
