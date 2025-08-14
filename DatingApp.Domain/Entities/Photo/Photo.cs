using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.Photo
{
    public class Photo
    {
        public int PhotoId { get; set; }

        public int UserId { get; set; }

        public string? Url { get; set; }

        public bool IsMain { get; set; }

        public string? PublicId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User.User? User { get; set; }
    }
}
