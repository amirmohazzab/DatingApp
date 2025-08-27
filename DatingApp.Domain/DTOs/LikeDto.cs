using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class LikeDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string KnownAs { get; set; }

        public string PictureUrl { get; set; }

        public string City { get; set; }

    }
}
