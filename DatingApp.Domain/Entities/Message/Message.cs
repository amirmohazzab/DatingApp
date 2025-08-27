using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.Message
{
    public class Message
    {
        public int MessageId { get; set; }

        public int SenderId { get; set; }

        public string SenderUserName { get; set; }

        public int ReceiverId { get; set; }

        public string ReceiverUserName { get; set; }

        public string Content { get; set; }

        public DateTime DateRead { get; set; } 

        public DateTime MessageSent { get; set; } = DateTime.Now;

        public bool SenderDeleted { get; set; }

        public bool ReceiverDeleted { get; set; }

        public bool IsRead { get; set; } = false;

        public User.User Sender { get; set; }

        public User.User Receiver { get; set; }
    }
}
