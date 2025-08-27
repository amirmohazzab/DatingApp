using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.User
{
    public class UserLike
    {
        public int SourceUserId { get; set; }

        public int TargetUserId { get; set; }

        public User SourceUser { get; set; }

        public User TargetUser { get; set; }
    }
}
