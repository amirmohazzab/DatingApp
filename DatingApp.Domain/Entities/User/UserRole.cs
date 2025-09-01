using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.User
{
    public class UserRole : IdentityUserRole<int>
    {
        public User? User { get; set; }

        public Role.Role? Role { get; set; }
    }
}
