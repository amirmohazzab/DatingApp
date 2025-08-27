using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUserName { get; set; }

        public string Content { get; set; }
    }
}
