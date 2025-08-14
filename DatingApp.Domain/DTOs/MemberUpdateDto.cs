using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class MemberUpdateDto
    {
        public string? Email { get; set; }

        public string? KnownAs { get; set; }

        public string? Introduction { get; set; }

        public string? LookingFor { get; set; }

        public string? Interests { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? PhotoUrl { get; set; }

        public ICollection<PhotoDTO>? Photos { get; set; }
    }
}
