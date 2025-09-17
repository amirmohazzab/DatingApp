using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Application.SignalR
{
    public class PresenceTracker
    {
        private readonly Dictionary<string, List<string>> onlineUsers = new Dictionary<string, List<string>>();

        public Task UserConnected(string userName, string connectionId)
        {
            lock (onlineUsers) 
            {
                if (onlineUsers.ContainsKey(userName))
                {
                    onlineUsers[userName].Add(connectionId);
                }
                else
                {
                    onlineUsers.Add(userName, new List<string> { connectionId });
                }
            }

            return Task.CompletedTask;
        }

        public Task UserDisconnected(string userName, string connectionId)
        {
            lock (onlineUsers)
            {
                if (!onlineUsers.ContainsKey(userName)) return Task.CompletedTask;
                onlineUsers[userName].Remove(connectionId);
                if (onlineUsers[userName].Count == 0)
                    onlineUsers.Remove(userName);
            }
            return Task.CompletedTask;
        }

        public Task<string[]> GetOnlineUsers()
        {
            string[] UserOnline;
            lock (onlineUsers)
                UserOnline = onlineUsers.OrderBy(x => x.Key).Select(x => x.Key).ToArray();
            return Task.FromResult(UserOnline);
        }

        public Task<List<string>> GetConnectionsForUser(string userName)
        {
            List<string> connectionIds = new();
            lock (onlineUsers)
                connectionIds = onlineUsers.GetValueOrDefault(userName);
            return Task.FromResult(connectionIds);
        }  
            
    }
}
