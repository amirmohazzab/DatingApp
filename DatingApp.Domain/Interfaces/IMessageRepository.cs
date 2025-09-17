using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = DatingApp.Domain.Entities.User.Group;

namespace DatingApp.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task AddMessage(Message message);

        Task<Message> GetMessageById(int messageId);

        void DeleteMessage(Message message);

        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);

        Task<bool> SaveAll();

        Task UpdateMessageToRead(List<Message> messages, string userName);

        #region Signalr

        void AddGroup(Group group);

        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);

        Task<Group> GetMessageGroup(string groupName);

        #endregion
    }
}
