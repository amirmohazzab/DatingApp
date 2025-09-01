using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.Role
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
