using DatingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class UserTokenDTO
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Token { get; set; }

        public string? PhotoUrl { get; set; }

        public GenderEnum Gender { get; set; }

        public string? KnownAs { get; set; }

        public List<string>? Roles { get; set; }
    }
}
